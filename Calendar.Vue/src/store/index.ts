import { createStore } from 'vuex'

const existingStore = createStore({
  strict: process.env.NODE_ENV !== 'production',
  state: {
    loading: false,
    errorLoading: false,
  },
  mutations: {},
  actions: {},
  getters: {},
})

// Add the new store configuration
existingStore.registerModule('newStore', {
  state: {
    getUser: {},
  },
  mutations: {
    setUser(state, val) {
      state.getUser = val
    },
  },
  actions: {
    setUser({ commit }, val) {
      commit('setUser', val)
    },
  },
  getters: {
    getUser: (state) => state.getUser,
  },
})

export default existingStore
