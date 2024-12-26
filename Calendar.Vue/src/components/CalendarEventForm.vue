<template>
  <!-- START DIALOG -->
  <el-dialog
    v-model="dialog.calendarEventForm"
    :title="title"
    width="600"
    center
    :before-close="clear"
  >
    <el-form @submit.prevent="submitForm" label-position="top">
      <el-form-item label="Calendar">
        <el-select v-model="form.calendarId" placeholder="Calendar">
          <el-option
            v-for="calendar in calendars"
            :key="calendar.calendarId"
            :label="calendar.calendarName"
            :value="calendar.calendarId"
          />
        </el-select>
      </el-form-item>
      <el-form-item label="Event Name">
        <el-input v-model="form.eventName" placeholder="Event Name" />
      </el-form-item>
      <el-form-item label="Event Description">
        <el-input type="textarea" v-model="form.eventDescription" placeholder="Event Description" />
      </el-form-item>
      <el-form-item label="Event Color">
        <el-color-picker v-model="form.eventColor" :predefine="predefineColors" />
      </el-form-item>
      <el-form-item label="Is Recurring?">
        <el-radio-group v-model="form.isRecurring">
          <el-radio label="Yes" :value="true" />
          <el-radio label="No" :value="false" />
        </el-radio-group>
      </el-form-item>
      <el-form-item label="Date Range">
        <VDatePicker
          @dayclick="isDatePickerClicked = true"
          :popover="popover"
          v-model.range="form.dateRange"
          :columns="2"
          :first-day-of-week="1"
          mode="date"
        >
          <template #default="{ inputValue, inputEvents }">
            <div class="d-flex justify-content-center">
              <div class="row">
                <div class="col">
                  <el-input
                    :value="inputValue.start"
                    v-on="inputEvents.start"
                    placeholder="Start Date"
                  />
                </div>

                <div class="col">
                  <el-input :value="inputValue.end" v-on="inputEvents.end" placeholder="End Date" />
                </div>
              </div>
            </div>
          </template>
        </VDatePicker>
      </el-form-item>
      <div class="row">
        <el-form-item class="col-4 timePicker" label="Start Time">
          <el-time-picker format="HH:mm" v-model="form.startTime" placeholder="Start Time" />
        </el-form-item>
        <el-form-item class="col-4 timePicker" label="End Time">
          <el-time-picker format="HH:mm" v-model="form.endTime" placeholder="End Time" />
        </el-form-item>
      </div>
      <div v-if="form.isRecurring == true">
        <el-checkbox
          @change="selectedDay(day.value)"
          v-for="day in days"
          :key="day.value"
          :label="day.label"
          :value="day.value"
          size="large"
        />
      </div>
    </el-form>

    <template #footer>
      <div class="d-flex justify-content-end">
        <el-button type="primary" @click="submitForm">Confirm</el-button>
      </div>
    </template>
  </el-dialog>
  <!-- END DIALOG -->
</template>

<script>
import axios from 'axios'
import { ElMessage } from 'element-plus'
const api = import.meta.env.VITE_APP_API_URL
export default {
  props: {
    calendars: Array,
    openForm: Boolean,
    title: String,
    handleDateRange: Object,
    calendarEvent: Object,
  },
  data() {
    return {
      calendarEventId: '',
      calendarEventGroupId: '',
      calendarList: [],
      popover: {
        visibility: 'click',
      },
      dialog: {
        calendarEventForm: false,
      },
      form: {
        calendarId: '',
        eventName: '',
        eventDescription: '',
        eventColor: '#409EFF',
        isRecurring: false,
        startTime: '',
        endTime: '',
        dateRange: {
          start: '',
          end: '',
        },
        isDatePickerClicked: false,
      },
      // PREDEFINED COLORS
      predefineColors: [
        '#ff4500',
        '#ff8c00',
        '#ffd700',
        '#90ee90',
        '#00ced1',
        '#1e90ff',
        '#c71585',
        'rgba(255, 69, 0, 0.68)',
        'rgb(255, 120, 0)',
        'hsv(51, 100, 98)',
        'hsva(120, 40, 94, 0.5)',
        'hsl(181, 100%, 37%)',
        'hsla(209, 100%, 56%, 0.73)',
        '#c7158577',
      ],
    }
  },
  methods: {
    clear() {
      this.$emit('close')
      this.form.calendarId = ''
      this.form.eventName = ''
      this.form.eventDescription = ''
      this.form.eventColor = '#409EFF'
      this.form.isRecurring = false
      this.form.dateRange = { start: '', end: '' }
      this.form.startTime = ''
      this.form.endTime = ''
    },
    submitForm() {
      let startDate = new Date(this.form.dateRange.start)
      let endDate = new Date(this.form.dateRange.end)
      let startTime = new Date(this.form.startTime)
      let endTime = new Date(this.form.endTime)

      startTime.setHours(startTime.getHours() + 8)
      endTime.setHours(endTime.getHours() + 8)

      // RECURRING
      if (this.form.isRecurring == true) {
        let calendarEvents = []
        // Add 1 day in start and end date if VCalendar date is changed
        if (this.isDatePickerClicked == true) {
          startDate.setDate(startDate.getDate() + 1)
          endDate.setDate(endDate.getDate() + 1)
        }

        let newStartTime = startTime.toISOString().substr(11, 19)
        let newEndTime = endTime.toISOString().substr(11, 19)

        while (startDate <= endDate) {
          let newStartDate = startDate.toISOString().substr(0, 10)
          // let newEndDate = endDate.toISOString().substr(0, 10)

          calendarEvents.push({
            calendarId: this.form.calendarId,
            eventName: this.form.eventName,
            eventDescription: this.form.eventDescription,
            eventColor: this.form.eventColor,
            isRecurring: this.form.isRecurring,
            dateTimeStarted: `${newStartDate}T${newStartTime}`,
            dateTimeEnded: `${newStartDate}T${newEndTime}`,
          })
          startDate.setDate(startDate.getDate() + 1)
        }
        axios
          .post(`${api}/CalendarEvent/Recurring`, calendarEvents)
          .then(() => {
            ElMessage.success('Event added successfully')
            this.clear()
            this.$emit('refresh')
          })
          .catch((error) => {
            ElMessage.error(error)
          })
        return
      }
      // NON-RECURRING
      if (this.form.isRecurring == false) {
        // Add 1 day in start and end date if VCalendar date is changed
        if (this.isDatePickerClicked == true) {
          startDate.setDate(startDate.getDate() + 1)
          endDate.setDate(endDate.getDate() + 1)
        }

        let newStartDate = startDate.toISOString().substr(0, 10)
        let newEndDate = endDate.toISOString().substr(0, 10)
        let newStartTime = startTime.toISOString().substr(11, 19)
        let newEndTime = endTime.toISOString().substr(11, 19)

        const payload = {
          calendarId: this.form.calendarId,
          eventName: this.form.eventName,
          eventDescription: this.form.eventDescription,
          eventColor: this.form.eventColor,
          isRecurring: this.form.isRecurring,
          dateTimeStarted: `${newStartDate}T${newStartTime}`,
          dateTimeEnded: `${newEndDate}T${newEndTime}`,
        }

        axios
          .post(`${api}/CalendarEvent/NonRecurring`, payload)
          .then(() => {
            ElMessage.success('Event added successfully')
            this.clear()
            this.$emit('refresh')
          })
          .catch((error) => {
            ElMessage.error(error)
          })
        return
      }
    },
  },
  watch: {
    openForm(val) {
      this.dialog.calendarEventForm = val
    },
    handleDateRange(val) {
      console.log(val)
      this.form.calendarId = val.calendarId
      this.form.dateRange = {
        start: val.start,
        end: val.end,
      }
      this.form.startTime = `${val.start}T08:00:00`
      this.form.endTime = `2024-06-24T16:00:00`
    },
    calendars(val) {
      this.calendarList = val
    },
    calendarEvent(val) {
      this.calendarEventId = val.calendarEventId
      this.calendarEventGroupId = val.calendarEventGroupId
      this.form.calendarId = val.calendarId
      this.form.eventName = val.eventName
      this.form.eventDescription = val.eventDescription
      this.form.eventColor = val.eventColor
      this.form.isRecurring = val.isRecurring
      this.form.dateRange = {
        start: val.dateTimeStarted,
        end: val.dateTimeEnded,
      }

      this.form.startTime = val.dateTimeStarted // Extract "HH:mm"
      this.form.endTime = val.dateTimeEnded // Extract "HH:mm"
    },
  },
}
</script>
