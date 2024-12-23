<template>
  <el-container>
    <el-header class="border">Header</el-header>
    <el-container>
      <el-aside class="border sideMenu">
        <VCalendar
          style="width: 100%"
          v-model="today"
          @dayclick="dayClick"
          @did-move="didMove"
          :attributes="attributes"
      /></el-aside>
      <el-main class="main">
        <FullCalendar ref="refCalendar" :options="calendarOptions" />
      </el-main>
    </el-container>
  </el-container>
  <!-- START DIALOG -->
  <el-dialog v-model="dialog.eventForm" :title="title" width="600" center>
    <el-form @submit.prevent="submitForm" label-position="top">
      <el-form-item label="Calendar">
        <el-select v-model="form.calendarId" placeholder="Calendar">
          <el-option label="Calendar1" value="Calendar1" />
        </el-select>
      </el-form-item>
      <el-form-item label="Event Name">
        <el-input v-model="form.eventName" placeholder="Event Name" />
      </el-form-item>
      <el-form-item label="Is Recurring?">
        <el-radio-group v-model="form.isRecurring">
          <el-radio label="Yes" :value="true" />
          <el-radio label="No" :value="false" />
        </el-radio-group>
      </el-form-item>
      <el-form-item label="Date Range">
        <VDatePicker v-model.range="form.dateRange">
          <template #default="{ inputValue, inputEvents }">
            <div class="flex justify-center items-center">
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
        <el-form-item class="col-4" label="Start Time">
          <el-time-picker
            :disabled="form.dateRange.start == ''"
            @change="changeStartTime"
            v-model="form.timeStarted"
            placeholder="Start Time"
          />
        </el-form-item>
        <el-form-item class="col" label="End Time">
          <el-time-picker
            :disabled="form.dateRange.end == ''"
            @change="changeEndTime"
            v-model="form.timeEnded"
            placeholder="End Time"
          />
        </el-form-item>
      </div>
      <div v-if="form.isRecurring == true">
        <el-checkbox
          v-for="day in days"
          :key="day.id"
          :label="day.label"
          :value="day.id"
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
import FullCalendar from '@fullcalendar/vue3'
import rrulePlugin from '@fullcalendar/rrule'
import dayGridPlugin from '@fullcalendar/daygrid'
import interactionPlugin from '@fullcalendar/interaction'
import timeGridPlugin from '@fullcalendar/timegrid'
import list from '@fullcalendar/list'

export default {
  components: {
    FullCalendar,
  },
  data() {
    return {
      days: [
        { id: 1, label: 'Monday' },
        { id: 2, label: 'Tuesday' },
        { id: 3, label: 'Wednesday' },
        { id: 4, label: 'Thursday' },
        { id: 5, label: 'Friday' },
        { id: 6, label: 'Saturday' },
        { id: 7, label: 'Sunday' },
      ],
      currentMonth: '',
      currentYear: '',
      currentDay: '',
      firstDayOfMonth: '',
      lastDayOfMonth: '',
      title: '',
      form: {
        calendarId: '',
        eventName: '',
        isRecurring: false,
        dateStarted: '',
        dateEnded: '',
        timeStarted: '',
        timeEnded: '',
        dateRange: {
          start: '',
          end: '',
        },
      },
      dialog: {
        eventForm: false,
      },
      today: new Date(),
      calendarApi: '',
      calendarOptions: {
        datesSet: (info) => {
          this.datesSet(info)
        },
        headerToolbar: {
          start: 'todayCustom,prevCustom,nextCustom',
          center: 'title',
          end: 'createEvent monthCustom,weekCustom,dayCustom,listCustom',
        },
        // Custom Button
        customButtons: {
          createEvent: {
            text: 'Create Event',
            click: () => {
              this.createEvent()
            },
          },
          todayCustom: {
            text: 'today',
            click: () => {
              this.todayCustom()
            },
          },
          prevCustom: {
            text: 'prev',
            click: () => {
              this.prevCustom()
            },
          },
          nextCustom: {
            text: 'next',
            click: () => {
              this.nextCustom()
            },
          },
          monthCustom: {
            text: 'month',
            click: () => {
              this.monthCustom()
            },
          },
          weekCustom: {
            text: 'week',
            click: () => {
              this.weekCustom()
            },
          },
          dayCustom: {
            text: 'day',
            click: () => {
              this.dayCustom()
            },
          },
          listCustom: {
            text: 'list',
            click: () => {
              this.listCustom()
            },
          },
        },
        height: 600,
        plugins: [dayGridPlugin, interactionPlugin, timeGridPlugin, list, rrulePlugin],
        timeZone: 'UTC',
        events: [
          {
            title: 'Example',
            start: '2024-05-09',
            end: '2024-05-12',
            color: '#ff0000',
            lieu: 'Montreal',
            numero: '300445',
          },
        ],
        firstDay: 0, // 0 - [Mon, Tue, Wed, Thu, Fri, Sat, Sun] | 1 - [Sun, Mon, Tue, Wed, Thu, Fri, Sat]
        initialView: 'dayGridMonth', // Default fullcalendar view
        eventDrop: this.droppedEvent, // Drag and drop
        dateClick: this.handleClick, // click single date
        selectable: true, // Set selectable to true
        editable: true, // Set editable to true
        eventResizableFromStart: true, // Set resizable to true
        droppable: true, // Set drag and drop to true
        eventResize: this.resizeEvent, // Resize event
        select: this.handleDateRange, // Select multiple event
        eventClick: this.eventClick, // Click event
        allDaySlot: false, // Show/hide non-recurring events false
        eventLongPressDelay: 200, // Mobile drog n drop events
        eventOverlap: true,
        forceEventDuration: true,
        displayEventTime: true, // Display time for recurring event
        showNonCurrentDates: false, // Disable last week of previous month and first week of next month
      },
    }
  },
  computed: {
    attributes() {
      return [
        {
          highlight: {
            start: { fillMode: 'outline' },
            base: { fillMode: 'light' },
            end: { fillMode: 'outline' },
          },
          dates: { start: new Date(this.firstDayOfMonth), end: new Date(this.lastDayOfMonth) },
        },
      ]
    },
  },
  methods: {
    createEvent() {
      this.title = 'Create Event'
      this.dialog.eventForm = true
    },
    todayCustom() {
      this.today = new Date()
      this.calendarApi.today()
    },
    prevCustom() {
      this.calendarApi.prev()
      const currentDate = this.calendarApi.getDate()
      this.calendarApi.gotoDate(currentDate.toISOString())
    },
    nextCustom() {
      this.calendarApi.next()
      const currentDate = this.calendarApi.getDate()
      this.calendarApi.gotoDate(currentDate.toISOString())
    },
    monthCustom() {
      this.calendarApi.changeView('dayGridMonth')
    },
    weekCustom() {
      this.calendarApi.changeView('timeGridWeek')
    },
    dayCustom() {
      this.calendarApi.changeView('timeGridDay')
    },
    listCustom() {
      this.calendarApi.changeView('listMonth')
    },

    // METHODS
    datesSet(info) {
      let lastDay = new Date(info.view.activeEnd)
      lastDay.setDate(lastDay.getDate() - 1)
      this.firstDayOfMonth = info.view.activeStart.toISOString()
      this.lastDayOfMonth = lastDay.toISOString()
      this.currentMonth = info.view.activeStart.toISOString().substr(5, 2)
      this.currentYear = info.view.activeStart.toISOString().substr(0, 4)
    },
    didMove(info) {
      this.calendarApi.gotoDate(`${info[0].id}-01T00:00:00`)
    },
    dayClick(info) {
      console.log(info)
      this.calendarApi.changeView('timeGridDay')
      this.calendarApi.gotoDate(info.endDate)
    },
    submitForm() {
      const payload = {
        calendarId: this.form.calendarId,
        eventName: this.form.eventName,
        isRecurring: this.form.isRecurring,
        dateTimeStarted: this.form.dateRange.start,
        dateTimeEnded: this.form.dateRange.end,
      }
      console.log(payload)
    },
    changeStartTime(info) {
      const hours = info.toISOString().substr(11, 2)
      const minutes = info.toISOString().substr(14, 2)
      const seconds = info.toISOString().substr(17, 2)
      this.form.dateRange.start.setHours(hours, minutes, seconds)
    },
    changeEndTime(info) {
      const hours = info.toISOString().substr(11, 2)
      const minutes = info.toISOString().substr(14, 2)
      const seconds = info.toISOString().substr(17, 2)
      this.form.dateRange.end.setHours(hours, minutes, seconds)
    },

    handleDateRange: function (info) {
      alert('selected ' + info.startStr + ' to ' + info.endStr)
    },
  },
  mounted() {
    this.calendarApi = this.$refs.refCalendar.getApi()
  },
}
</script>

<style>
.main {
  min-height: 90vh;
}
.sideMenu {
  width: 250px;
}
</style>
