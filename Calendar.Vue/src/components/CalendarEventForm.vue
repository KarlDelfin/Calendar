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
          <el-time-picker
            :disabled="dayClicked || weekClicked"
            format="HH:mm"
            v-model="form.startTime"
            placeholder="Start Time"
          />
        </el-form-item>
        <el-form-item class="col-4 timePicker" label="End Time">
          <el-time-picker
            :disabled="dayClicked || weekClicked"
            format="HH:mm"
            v-model="form.endTime"
            placeholder="End Time"
            :disabled-hours="disabledEndHours"
          />
        </el-form-item>
      </div>
      <div v-if="form.isRecurring">
        <el-divider content-position="left">Recurring Every</el-divider>

        <!-- <div class="row d-flex pt-4">
          <div class="col" v-for="(day, index) in days" :key="index">
            <input
              class="weekDayClass"
              type="checkbox"
              :id="'checkbox-' + day.label"
              :value="day.value"
              @change="selectedDay(day.value)"
              :checked="day.checked"
            />
            <label class="px-1">{{ day.label }}</label>
          </div>
        </div> -->
        <el-checkbox
          v-for="(day, index) in days"
          :key="index"
          @change="selectedDay(day.value)"
          :label="day.label"
          :value="day.value"
          size="large"
          :checked="day.checked"
        >
          {{ day.label }}
        </el-checkbox>
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
import { v4 as uuidv4 } from 'uuid'

export default {
  props: {
    calendars: Array,
    openForm: Boolean,
    title: String,
    handleDateRange: Object,
    calendarEvent: Object,
    selectedDayList: Array,
  },
  data() {
    return {
      dayClicked: false,
      weekClicked: false,
      actionStatus: '',
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
        isDayAdded: true,
      },
      selectedDays: [],
      days: [
        { label: 'Sun', value: 0, checked: false },
        { label: 'Mon', value: 1, checked: false },
        { label: 'Tue', value: 2, checked: false },
        { label: 'Wed', value: 3, checked: false },
        { label: 'Thu', value: 4, checked: false },
        { label: 'Fri', value: 5, checked: false },
        { label: 'Sat', value: 6, checked: false },
      ],
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
    // DISABLE PREVIOUS HOURS SELECTED BY START TIME
    disabledEndHours() {
      const startHour = this.form.startTime ? new Date(this.form.startTime).getHours() : null

      if (startHour !== null) {
        return Array.from({ length: 24 }, (_, i) => i).filter((hour) => hour <= startHour)
      }

      return []
    },
    selectedDay(e) {
      const index = this.selectedDays.indexOf(e)
      if (index !== -1) {
        this.selectedDays.splice(index, 1)
      } else {
        this.selectedDays.push(e)
      }
    },
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
      this.selectedDays.length = 0
      this.selectedDays = []
      this.isDatePickerClicked = false
      this.isDayAdded = true
      this.dayClicked = false
      this.weekClicked = false
    },
    submitForm() {
      if (
        this.form.eventName == '' ||
        this.form.eventDescription == '' ||
        this.form.eventColor == ''
      ) {
        ElMessage.info('Please input fields!')
        return
      }
      let calendarEvents = []
      let calendarEventGroupId = uuidv4()
      let startDate = new Date(this.form.dateRange.start)
      let endDate = new Date(this.form.dateRange.end)
      let startTime = new Date(this.form.startTime)
      let endTime = new Date(this.form.endTime)
      startTime.setHours(startTime.getHours() + 8)
      endTime.setHours(endTime.getHours() + 8)
      // CREATE CALENDAR EVENT
      if (this.title == 'Create Event') {
        // RECURRING (WITH SELECTED DAYS)
        if (this.form.isRecurring == true && this.selectedDays.length > 0) {
          while (startDate <= endDate) {
            let newStartDate = new Date(startDate)
            const startDay = startDate.getDay()

            if (this.selectedDays.includes(startDay)) {
              // Add 1 day if VCalendar is changed
              if (
                (this.isDatePickerClicked == true && this.isDayAdded == false) ||
                (this.isDatePickerClicked == false && this.isDayAdded == false)
              ) {
                newStartDate.setDate(newStartDate.getDate())
              } else {
                newStartDate.setDate(newStartDate.getDate() + 1)
              }

              let newStartTime = startTime.toISOString().substr(11, 19)
              let newEndTime = endTime.toISOString().substr(11, 19)

              calendarEvents.push({
                calendarId: this.form.calendarId,
                eventName: this.form.eventName,
                eventDescription: this.form.eventDescription,
                eventColor: this.form.eventColor,
                isRecurring: this.form.isRecurring,
                dateTimeStarted: `${newStartDate.toISOString().substr(0, 10)}T${newStartTime}`,
                dateTimeEnded: `${newStartDate.toISOString().substr(0, 10)}T${newEndTime}`,
                calendarEventGroupId: calendarEventGroupId,
              })
            }
            startDate.setDate(startDate.getDate() + 1)
          }
        }
        // RECURRING
        if (this.form.isRecurring == true) {
          if (this.isDatePickerClicked == true) {
            startDate.setDate(startDate.getDate() + 1)
            endDate.setDate(endDate.getDate() + 1)
          }
          let newStartTime = startTime.toISOString().substr(11, 19)
          let newEndTime = endTime.toISOString().substr(11, 19)

          while (startDate <= endDate) {
            let newStartDate = startDate.toISOString().substr(0, 10)

            calendarEvents.push({
              calendarId: this.form.calendarId,
              eventName: this.form.eventName,
              eventDescription: this.form.eventDescription,
              eventColor: this.form.eventColor,
              isRecurring: this.form.isRecurring,
              dateTimeStarted: `${newStartDate}T${newStartTime}`,
              dateTimeEnded: `${newStartDate}T${newEndTime}`,
              calendarEventGroupId: calendarEventGroupId,
            })
            startDate.setDate(startDate.getDate() + 1)
          }
        }
        // NON-RECURRING
        if (this.form.isRecurring == false) {
          if (this.isDatePickerClicked == true) {
            startDate.setDate(startDate.getDate() + 1)
            endDate.setDate(endDate.getDate() + 1)
          }
          let newStartDate = startDate.toISOString().substr(0, 10)
          let newEndDate = endDate.toISOString().substr(0, 10)
          let newStartTime = startTime.toISOString().substr(11, 19)
          let newEndTime = endTime.toISOString().substr(11, 19)

          calendarEvents.push({
            calendarId: this.form.calendarId,
            eventName: this.form.eventName,
            eventDescription: this.form.eventDescription,
            eventColor: this.form.eventColor,
            isRecurring: this.form.isRecurring,
            dateTimeStarted: `${newStartDate}T${newStartTime}`,
            dateTimeEnded: `${newEndDate}T${newEndTime}`,
            calendarEventGroupId: null,
          })
        }

        axios
          .post(`${api}/CalendarEvent`, calendarEvents)
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
      // UPDATE CALENDAR EVENT
      else {
        if (this.calendarEventGroupId == null) {
          this.calendarEventGroupId = '00000000-0000-0000-0000-000000000000'
        }

        // RECURRING (WITH SELECTED DAYS)
        if (this.form.isRecurring == true && this.selectedDays.length > 0) {
          while (startDate <= endDate) {
            let newStartDate = new Date(startDate)
            const startDay = startDate.getDay()

            if (this.selectedDays.includes(startDay)) {
              let newStartTime = startTime.toISOString().substr(11, 19)
              let newEndTime = endTime.toISOString().substr(11, 19)

              calendarEvents.push({
                calendarId: this.form.calendarId,
                eventName: this.form.eventName,
                eventDescription: this.form.eventDescription,
                eventColor: this.form.eventColor,
                isRecurring: this.form.isRecurring,
                dateTimeStarted: `${newStartDate.toISOString().substr(0, 10)}T${newStartTime}`,
                dateTimeEnded: `${newStartDate.toISOString().substr(0, 10)}T${newEndTime}`,
                calendarEventGroupId: calendarEventGroupId,
              })
            }
            startDate.setDate(startDate.getDate() + 1)
          }
        }
        // RECURRING
        if (this.form.isRecurring == true) {
          let newStartTime = startTime.toISOString().substr(11, 19)
          let newEndTime = endTime.toISOString().substr(11, 19)

          while (startDate <= endDate) {
            let newStartDate = startDate.toISOString().substr(0, 10)

            calendarEvents.push({
              calendarId: this.form.calendarId,
              eventName: this.form.eventName,
              eventDescription: this.form.eventDescription,
              eventColor: this.form.eventColor,
              isRecurring: this.form.isRecurring,
              dateTimeStarted: `${newStartDate}T${newStartTime}`,
              dateTimeEnded: `${newStartDate}T${newEndTime}`,
              calendarEventGroupId: calendarEventGroupId,
            })
            startDate.setDate(startDate.getDate() + 1)
          }
        }
        // NON-RECURRING
        if (this.form.isRecurring == false) {
          let newStartDate = startDate.toISOString().substr(0, 10)
          let newEndDate = endDate.toISOString().substr(0, 10)
          let newStartTime = startTime.toISOString().substr(11, 19)
          let newEndTime = endTime.toISOString().substr(11, 19)

          calendarEvents.push({
            calendarId: this.form.calendarId,
            eventName: this.form.eventName,
            eventDescription: this.form.eventDescription,
            eventColor: this.form.eventColor,
            isRecurring: this.form.isRecurring,
            dateTimeStarted: `${newStartDate}T${newStartTime}`,
            dateTimeEnded: `${newEndDate}T${newEndTime}`,
            calendarEventGroupId: null,
          })
        }

        axios
          .put(
            `${api}/CalendarEvent?calendarEventId=${this.calendarEventId}&calendarEventGroupId=${this.calendarEventGroupId}&actionStatus=${this.actionStatus}`,
            calendarEvents,
          )
          .then(() => {
            ElMessage.success('Event updated successfully')
            this.clear()
            this.$emit('refresh')
          })
          .catch((error) => {
            ElMessage.error(error)
          })
      }
    },
  },
  watch: {
    openForm(val) {
      this.dialog.calendarEventForm = val
    },
    handleDateRange(val) {
      this.dayClicked = val.dayClicked
      this.weekClicked = val.weekClicked
      var newStartTime = new Date(val.start)
      var newEndTime = new Date(val.end)
      newStartTime.setHours(newStartTime.getHours() - 8)
      newEndTime.setHours(newEndTime.getHours() - 8)
      this.form.calendarId = val.calendarId
      this.form.dateRange = {
        start: val.start,
        end: val.end,
      }
      if (val.weekClicked == false && val.dayClicked == false) {
        this.form.startTime = `${val.start}T08:00:00`
        this.form.endTime = `2024-06-24T16:00:00`
      } else {
        this.form.startTime = newStartTime
        this.form.endTime = newEndTime
      }

      this.isDatePickerClicked = false
      this.isDayAdded = false
    },
    calendars(val) {
      this.calendarList = val
    },
    calendarEvent(val) {
      this.actionStatus = val.status
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
      this.form.startTime = val.dateTimeStarted
      this.form.endTime = val.dateTimeEnded
    },
    selectedDayList(val) {
      for (let i = 0; i < val.length; i++) {
        this.selectedDay(val[i])
      }
      this.days.forEach((day) => {
        day.checked = val.includes(day.value)
      })
    },
  },
}
</script>
