import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new  Vuex.Store({
    state: {
        targets: [],
        agents: []
    },
    mutations: {
        targets(state, targets) {
            state.targets = targets
        },
        createTarget(state, target) {
            state.targets.push(target)
        },
        updateTarget(state, target) {
            state.targets.forEach((t, i) => {
                if (t.id === target.id) {
                    state.targets[i].name = target.name
                    state.targets[i].rootDomain = target.rootDomain
                    state.targets[i].rootDomain = target.rootDomain
                    state.targets[i].bugBountyProgramUrl = target.bugBountyProgramUrl
                    state.targets[i].isPrivate = target.isPrivate
                    state.targets[i].inScope = target.inScope
                    state.targets[i].outOfScope = target.outOfScope
                }
            });
        },
        deleteTarget(state, target) {
            state.targets = state.targets.filter((t) => {
                return t.name !== target.name;
            })
        },
        agents(state, agents) {
            state.agents = agents
        },
        createAgent(state, agent) {
            state.agents.push(agent)
        },
        updateAgent(state, agent) {
            state.agents.forEach((a, i) => {
                if (a.id === agent.id) {
                    state.agents[i].name = agent.name
                    state.agents[i].categories = agent.categories
                    state.agents[i].command = agent.command
                    state.agents[i].isBySubdomain = agent.isBySubdomain
                    state.agents[i].onlyIfIsAlive = agent.onlyIfIsAlive
                    state.agents[i].onlyIfHasHttpOpen = agent.onlyIfHasHttpOpen
                    state.agents[i].skipIfRanBefore = agent.skipIfRanBefore
                    state.agents[i].script = agent.script;
                }
            });
        },
        deleteAgent(state, agent) {
            console.log(agent)
            state.agents = state.agents.filter((a) => {
                return a.name !== agent.name;
            })
        },
    },
    actions: {
        async targets(context, api) {
            const targets = (await api.get('targets')).data 
            context.commit('targets', targets)
        },
        createTarget(context, { api, target }) {
            return new Promise((resolve, reject) => {
                try {
                    api.create('targets', target)
                        .then(() => {
                            context.commit('createTarget', target)
                            resolve()
                        })
                        .catch(error => reject(error))
                }
                catch {
                    reject()
                }
            })    
        },
        updateTarget(context, { api, target }) {
            return new Promise((resolve, reject) => {
                try {
                    api.update('targets', target.id, target)
                        .then(() => {
                            context.commit('updateTarget', target)
                            resolve()
                        })   
                        .catch(error => reject(error))
                }
                catch {
                    reject()
                }
            })
        },
        deleteTarget(context, { api, target }) {
            return new Promise((resolve, reject) => {
                try {
                    api.delete('targets', target.name)
                        .then(() => {
                            context.commit('deleteTarget', target)
                            resolve()
                        })
                        .catch(error => reject(error))
                }
                catch {
                    reject()
                }
            })
        },
        async agents(context, api) {
            const agents = (await api.get('agents')).data
            context.commit('agents', agents)
        },
        createAgent(context, { api, agent }) {
            return new Promise((resolve, reject) => {
                try {
                    api.create('agents', agent)
                        .then(() => {
                            context.commit('createAgent', agent)
                            resolve()
                        })
                        .catch(error => reject(error))
                }
                catch {
                    reject()
                }
            })
        },
        updateAgent(context, { api, agent }) {
            return new Promise((resolve, reject) => {
                try {
                    api.update('agents', agent.id, agent)
                        .then(() => {
                            context.commit('updateAgent', agent)
                            resolve()
                        })
                        .catch(error => reject(error))
                }
                catch {
                    reject()
                }
            })
        },
        deleteAgent(context, { api, agent }) {
            return new Promise((resolve, reject) => {
                try {
                    api.delete('agents', agent.name)
                        .then(() => {
                            context.commit('deleteAgent', agent)
                            resolve()
                        })
                        .catch(error => reject(error))
                }
                catch {
                    reject()
                }
            })
        },
    }
})