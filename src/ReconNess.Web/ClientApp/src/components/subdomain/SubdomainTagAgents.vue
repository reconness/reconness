<template>
  <div class="pt-2 row">
    <div class="mx-auto"><strong>List of Agents</strong></div>

    <div class="col-12">
      <table class="table table-striped">
        <thead class="thead-dark">
          <tr>
            <th scope="col">Name</th>
            <th scope="col">Categories</th>
            <th scope="col">Last Run</th>
            <th scope="col">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="agent in agents" v-bind:key="agent.id">
            <th class="w-25" scope="row">{{ agent.name }}</th>
            <td class="w-25">{{ agent.categories.join(', ') }}</td>
            <td class="w-25">{{ agent.lastRun | formatDate('YYYY-MM-DD') }}</td>
            <td class="w-25">
              <button class="btn btn-primary ml-2" v-on:click="onRunAgent(agent)" v-if="!agent.isRunning" :disabled="disabledCanRun(agent)">Run</button>
              <button class="btn btn-danger ml-2" v-on:click="onStopAgent(agent)" v-if="agent.isRunning">Stop</button>
              <button class="btn btn-dark ml-2" v-on:click="showTerminalModal = !showTerminalModal" v-if="agent.isRunning">Terminal</button>
              <button class="btn btn-dark ml-2" v-on:click="showLogModal = !showLogModal" v-if="agent.isRunning">Logs</button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <div class="terminalModal">
      <!-- Modal-->
      <transition @enter="startTransitionModal" @after-enter="endTransitionModal" @before-leave="endTransitionModal" @after-leave="startTransitionModal">
        <div class="modal fade" v-if="showTerminalModal" ref="modal">
          <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title" id="exampleModalLabel">Terminal</h5>
                <button class="close" type="button" v-on:click="showTerminalModal = !showTerminalModal"><span aria-hidden="true">x</span></button>
              </div>
              <div class="modal-body">
                <div id="terminal"></div>
              </div>
            </div>
          </div>
        </div>
      </transition>
      <div class="modal-backdrop fade d-none" ref="backdrop"></div>
    </div>

    <div class="logModal">
      <!-- Modal-->
      <transition @enter="startTransitionLogModal" @after-enter="endTransitionLogModal" @before-leave="endTransitionLogModal" @after-leave="startTransitionLogModal">
        <div class="modal fade" v-if="showLogModal" ref="modal">
          <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
              <div class="modal-header">
                <h5 class="modal-title" id="exampleModalLabel">Logs</h5>
                <button class="close" type="button" v-on:click="showLogModal = !showLogModal"><span aria-hidden="true">x</span></button>
              </div>
              <div class="modal-body">
                <div id="log"></div>
              </div>
            </div>
          </div>
        </div>
      </transition>
      <div class="modal-backdrop fade d-none" ref="backdrop"></div>
    </div>

  </div>
</template>

<script>
  import { Terminal } from 'xterm'

  export default {
    name: 'SubdomainTagAgents', 
    props: {
      agents: {
        type: Array,
        required: true
      },
      subdomain: {
        type: Object,
        required: true
      }
    },
    data() {
      return { 
        showTerminalModal: false,
        showLogModal: false,
        term: null,
        termLog: null,
        agentRunning: null
      }    
    },    
    async mounted() {
      if (this.agents.length > 0) {
        this.agents.map(agent => {
          const channel = `${this.$route.params.targetName}_${this.$route.params.subdomain}_${agent.name}`
          this.$connection.on(channel, (message) => {
            if (message === "Agent stopped!" || message === "Agent done!") {
              agent.isRunning = false;
              this.agentRunning = null
            }

            if (this.term !== null) {
              this.term.writeln(message)
            }
          });

          this.$connection.on("logs_" + channel, (message) => {

            if (this.termLog !== null) {
              this.termLog.writeln(message)
            }
          });
        })
      }
    },    
    methods: {  
      async onRunAgent(agent) {
        if (agent.isRunning) {
          return
        }

        agent.isRunning = true
        this.showTerminalModal = true
        this.agentRunning = agent

        const target = this.$route.params.targetName
        const subdomainName = this.$route.params.subdomain
        const agentName = agent.name      

        await this.$api.create('agents/run', {
          agent: agentName,
          target: target,
          subdomain: subdomainName
        })      
      },
      async onStopAgent(agent) { 
        if (!agent.isRunning) {
          return
        }

        this.agentRunning = null

        const target = this.$route.params.targetName
        const subdomainName = this.$route.params.subdomain
        const agentName = agent.name

        await this.$api.create('agents/stop', {
          agent: agentName,
          target: target,
          subdomain: subdomainName
        })      
      },
      disabledCanRun(agent) {
        const anotherAgentIsRunning = this.agentRunning != null && this.agentRunning.name !== agent.name
        const needToBeAlive = agent.onlyIfIsAlive === true && this.subdomain.isAlive !== true

        return needToBeAlive || anotherAgentIsRunning
      },
      startTransitionModal() {      
        this.$refs.backdrop.classList.toggle("d-block");
        if (this.$refs.modal !== undefined) {
          this.$refs.modal.classList.toggle("d-block");

          this.term = new Terminal({
            disableStdin: true
          })
          this.term.open(document.getElementById('terminal'))
        }       
      },      
      endTransitionModal() {
        this.$refs.backdrop.classList.toggle("show");
        if (this.$refs.modal !== undefined) {
          this.$refs.modal.classList.toggle("show");
        }
      },
      startTransitionLogModal() {      
        this.$refs.backdrop.classList.toggle("d-block");
        if (this.$refs.modal !== undefined) {
          this.$refs.modal.classList.toggle("d-block");

          this.termLog = new Terminal({
            disableStdin: true
          })
          this.termLog.open(document.getElementById('log'))
        }       
      },
      endTransitionLogModal() {
        this.$refs.backdrop.classList.toggle("show");
        if (this.$refs.modal !== undefined) {
          this.$refs.modal.classList.toggle("show");
        }
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
  .modal-dialog {
    max-width: 800px !important;
  }
</style>