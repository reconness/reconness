import api from '../../api'

const state = {
    agents: [],
    currentAgent: {}
}

const getters = {
    subdomainAgents: state => {
        return state.agents.filter(agent => agent.isBySubdomain)
    },
    installed: state => (agent) => {
        return state.agents.some(a => a.name === agent.name)
    }
}

const actions = {
    agent(context, agentName) {
        return new Promise((resolve, reject) => {
            try {
                api.getById('agents', agentName)
                    .then((res) => {
                        context.commit('agent', res.data)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    agents(context) {
        return new Promise((resolve, reject) => {
            try {
                api.get('agents')
                    .then((res) => {
                        context.commit('agents', res.data)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    runningAgents(context, { targetName, rootDomain, subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.get('agents/running/' + targetName + '/' + rootDomain + '/' + subdomain)
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    agentsMarketplace() {
        return new Promise((resolve, reject) => {
            try {
                api.get('agents/marketplace')
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    install(context, agentDefault) {
        return new Promise((resolve, reject) => {
            try {
                api.create('agents/install', agentDefault)
                    .then((res) => {
                        context.commit('createAgent', res.data)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    createAgent(context, agent) {
        return new Promise((resolve, reject) => {
            try {
                api.create('agents', agent)
                    .then(() => {
                        context.commit('createAgent', agent)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    updateAgent({ commit, state }) {
        return new Promise((resolve, reject) => {
            try {
                api.update('agents', state.currentAgent.id, state.currentAgent)
                    .then(() => {
                        commit('updateAgent', state.currentAgent)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    deleteAgent({ commit, state }) {
        return new Promise((resolve, reject) => {
            try {
                api.delete('agents', state.currentAgent.name)
                    .then(() => {
                        commit('deleteAgent', state.currentAgent)
                        resolve()
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
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
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
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
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    run(context, { agent, command, target, rootDomain, subdomain, activateNotification }) {
        return new Promise((resolve, reject) => {
            try {
                api.create('agents/run', { agent, command, target, rootDomain, subdomain, activateNotification })
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    },
    stop(context, { agent, target, rootDomain, subdomain }) {
        return new Promise((resolve, reject) => {
            try {
                api.create('agents/stop', { agent, target, rootDomain, subdomain })
                    .then((res) => {
                        resolve(res.data)
                    })
                    .catch(err => reject(err))
            }
            catch (err) {
                reject(err)
            }
        })
    }
}

const mutations = {
    agent(state, agent) {
        state.currentAgent = agent
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