<template>
  <el-header v-if="this.$route.path != '/'" class="border">
    <el-menu class="el-menu-demo d-flex justify-content-end" mode="horizontal">
      <el-sub-menu index="2">
        <template #title>Profile</template>
        <el-menu-item @click="logout"><i class="bi bi-power me-1"></i> Logout</el-menu-item>
      </el-sub-menu>
    </el-menu></el-header
  >
  <RouterView />
</template>

<script>
import { ElMessage } from 'element-plus'

export default {
  methods: {
    logout() {
      localStorage.clear()
      this.$router.push('/')
      ElMessage.success('You have been logout')
    },
  },
}
</script>
