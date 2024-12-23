<template>
  <el-container>
    <el-header class="border">Header</el-header>
    <el-container>
      <el-aside class="border sideMenu">Aside</el-aside>
      <el-main class="main">
        <RouterView />
      </el-main>
    </el-container>
  </el-container>
</template>

<script></script>

<style>
.main {
  min-height: 90vh;
}
.sideMenu {
  width: 200px;
}
</style>
