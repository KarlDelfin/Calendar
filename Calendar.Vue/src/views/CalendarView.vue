<template>
  <div class="m-3">
    <el-card>
      <div class="row">
        <div class="col d-flex justify-content-start">
          <el-button @click="this.$router.push('/home')">Back</el-button>
        </div>
        <div class="col d-flex justify-content-end">
          <el-button type="primary" @click="openForm('Create Calendar', [])"
            >Create Calendar</el-button
          >
        </div>
      </div>
      <div class="m-3"><h3>Calendar</h3></div>
      <el-scrollbar height="400px">
        <div v-if="calendars.length == 0">
          <el-empty description="No Data" />
        </div>
        <div v-else>
          <el-table :data="calendars">
            <el-table-column label="Calendar Name" prop="calendarName" />
            <el-table-column label="Operation">
              <template #default="scope">
                <el-button size="small" @click="openForm('Edit Calendar', scope.row)"
                  >Edit</el-button
                >
                <el-button size="small" type="danger" @click="deleteCalendar(scope.row.calendarId)"
                  >Delete</el-button
                >
              </template>
            </el-table-column>
          </el-table>
        </div>
      </el-scrollbar>
    </el-card>
    <el-card class="mt-2">
      <el-pagination
        v-model:current-page="calendarPagination.currentPage"
        v-model:page-size="calendarPagination.elementsPerPage"
        :page-sizes="[5, 10, 25, 50]"
        :total="calendarPagination.totalElements"
        layout="total, sizes, prev, pager, next, jumper"
        @size-change="getCalendarByUserId"
        @current-change="getCalendarByUserId"
      />
    </el-card>
  </div>

  <CalendarForm
    :openForm="dialog.calendarForm"
    :title="title"
    :calendarData="calendarData"
    @close="dialog.calendarForm = false"
    @refresh="getCalendarByUserId(user.userId)"
  />
</template>
<script>
import axios from 'axios'
import { ElMessage, ElMessageBox, ElLoading } from 'element-plus'
import CalendarForm from '../components/CalendarForm.vue'
const api = import.meta.env.VITE_APP_API_URL
export default {
  components: { CalendarForm },
  data() {
    return {
      title: '',
      calendarData: {},
      dialog: {
        calendarForm: false,
      },

      calendarPagination: {
        currentPage: 1,
        elementsPerPage: 10,
        totalElements: 0,
      },

      user: {},
      calendars: [],
    }
  },
  methods: {
    openForm(title, data) {
      this.title = title
      this.dialog.calendarForm = true

      if (title == 'Edit Calendar') {
        this.calendarData = data
      }
    },

    deleteCalendar(calendarId) {
      ElMessageBox.confirm('Do you want to delete this calendar?', 'Warning', {
        confirmButtonText: 'Confirm',
        cancelButtonText: 'Cancel',
        type: 'warning',
      })
        // CONFIRM
        .then(() => {
          axios
            .delete(`${api}/Calendar/${calendarId}`)
            .then(() => {
              ElMessage.success('Calendar deleted successfully')
              this.getCalendarByUserId(this.user.userId)
            })
            .catch((error) => {
              ElMessage.error(error.response.data)
            })
        })
        // CANCEL
        .catch(() => {})
    },
    getCalendarByUserId() {
      const loading = ElLoading.service({
        lock: true,
        text: 'Loading',
        background: 'rgba(0, 0, 0, 0.7)',
      })
      axios
        .get(
          `${api}/Calendar/User/${this.user.userId}?currentPage=${this.calendarPagination.currentPage}&elementsPerPage=${this.calendarPagination.elementsPerPage}`,
        )
        .then((response) => {
          this.calendars = response.data.results
          this.calendarPagination.totalElements = response.data.totalElements
          loading.close()
        })
        .catch((error) => {
          console.log(error)
        })
    },
  },

  mounted() {
    this.user = JSON.parse(localStorage.getItem('user'))
    console.log(this.user)
    if (this.user == null) {
      this.$router.push('/')
    }
    this.getCalendarByUserId(this.user.userId)
  },
}
</script>
