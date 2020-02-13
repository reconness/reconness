import api from '../../api'

const state = {
    agents: []
}

const getters = {
    subdomainAgents: state => {
        console.log(state.agents)
        return state.agents.filter(agent => agent.isBySubdomain)
    }
}

const actions = {
    agent(context, agentName) {
        return new Promise((resolve, reject) => {
            try {
                api.getById('agents', agentName)
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },    
    async agents(context) {
        const agents = (await api.get('agents')).data
        context.commit('agents', agents)
    },
    createAgent(context, agent) {
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
    updateAgent(context, agent ) {
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
    deleteAgent(context, agent) {
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
    categories() {
        return new Promise((resolve, reject) => {
            try {
                api.get('categories')
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
    debug(context, { terminalOutput, script }) {
        return new Promise((resolve, reject) => {
            try {
                api.create('agents/debug', { terminalOutput, script })
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
    run(context, { agent, command, target, subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.create('agents/run', {agent, command, target, subdomain }) 
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    },
    stop(context, { agent, target, subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.create('agents/stop', { agent, target, subdomain })
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(error => reject(error))
            }
            catch {
                reject()
            }
        })
    }
}

const mutations = {
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
}

export default {
    namespaced: true,
    state,
    getters,
    actions,
    mutations
}