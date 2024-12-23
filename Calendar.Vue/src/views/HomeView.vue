<template>
  <FullCalendar ref="refCalendar" :options="calendarOptions" />
  <!-- START DIALOG -->
  <el-dialog v-model="dialog.calendarForm" :title="title" center>
    <el-form @submit.prevent="submitForm" label-position="top">
      <el-form-item label="Calendar">
        <el-select v-model="form.calendarId" placeholder="Calendar">
          <el-option label="Calendar1" value="Calendar1" />
        </el-select>
      </el-form-item>
      <el-form-item label="Event Name">
        <el-input v-model="form.eventName" placeholder="Event Name" />
      </el-form-item>
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
      title: '',
      form: {
        calendarId: '',
        eventName: '',
        dateStarted: '',
        dateEnded: '',
        timeStarted: '',
        timeEnded: '',
      },
      dialog: {
        calendarForm: false,
      },
      today: new Date(),
      calendarApi: '',
      calendarOptions: {
        headerToolbar: {
          start: 'todayCustom,prevCustom,nextCustom',
          center: 'title',
          end: 'createEvent monthCustom,weekCustom,dayCustom,listCustom',
        },
        // Custom Button
        customButtons: {
          // Create Event
          createEvent: {
            text: 'Create Event',
            click: () => {
              this.createEvent()
            },
          },
          // Today
          todayCustom: {
            text: 'today',
            click: () => {
              this.todayCustom()
            },
          },
          // Previous Month
          prevCustom: {
            text: 'prev',
            click: () => {
              this.prevCustom()
            },
          },
          // Next Month
          nextCustom: {
            text: 'next',
            click: () => {
              this.nextCustom()
            },
          },

          // Month View
          monthCustom: {
            text: 'month',
            click: () => {
              this.monthCustom()
            },
          },
          // Week View
          weekCustom: {
            text: 'week',
            click: () => {
              this.weekCustom()
            },
          },
          // Day View
          dayCustom: {
            text: 'day',
            click: () => {
              this.dayCustom()
            },
          },
          // List View
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
  methods: {
    createEvent() {
      this.title = 'Create Event'
      this.dialog.calendarForm = true
    },
    todayCustom() {
      this.today = new Date()
      this.calendarApi.today()
    },
    prevCustom() {
      this.calendarApi.prev()
    },
    nextCustom() {
      this.calendarApi.next()
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
    submitForm() {
      console.log('AW')
    },

    handleDateClick: function (arg) {
      alert('date click! ' + arg.dateStr)
    },
    handleDateRange: function (info) {
      alert('selected ' + info.startStr + ' to ' + info.endStr)
    },
  },
  mounted() {
    this.calendarApi = this.$refs.refCalendar.getApi()
    console.log(this.$refs.refCalendar) // Check if the ref is correctly set
  },
}
</script>
