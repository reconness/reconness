<template>
  <div class="pt-2">
    <div class="form-group">
      <label for="inputName">Name</label>
      <input name="agentName" formControlName="agentName" class="form-control" id="agentName" v-model="agent.name">
    </div>
    <div class="form-group">
      <label for="inputCategory">Category</label>
      <vue-tags-input v-model="tag" placeholder="Add Category" :tags="tags" :autocomplete-items="filteredItems" @tags-changed="newTags => tags = newTags" />
    </div>
    <div class="form-group">
      <label for="inputCmd">Command <a href="https://docs.reconness.com/agents/add-agent#add-new-agent">Learn more</a></label>
      <input name="command" formControlName="command" class="form-control" id="command" v-model="agent.command">
    </div>
    <div class="form-group form-check">
      <input class="form-check-input" type="checkbox" id="isBySubdomain" v-model="agent.isBySubdomain" v-on:click="onBySubdomain()">
      <label class="form-check-label" for="isBySubdomain">
        Run by Subdomains
      </label>
    </div>
    <div class="form-group form-check">
      <input class="form-check-input" type="checkbox" id="onlyIfIsAlive" ref="onlyIfIsAlive" v-model="agent.onlyIfIsAlive" :disabled="disabledIsNotBySubdomain">
      <label class="form-check-label" for="onlyIfIsAlive">
        Run Only if it is Alive
      </label>
    </div>
    <div class="form-group form-check">
      <input class="form-check-input" type="checkbox" id="onlyIfHasHttpOne" ref="onlyIfHasHttpOpen" v-model="agent.onlyIfHasHttpOpen" :disabled="disabledIsNotBySubdomain">
      <label class="form-check-label" for="onlyIfHasHttpOpen">
        Run Only if has Http Open
      </label>
    </div>
    <div class="form-group form-check">
      <input class="form-check-input" type="checkbox" id="skipIfRanBefore" ref="skipIfRanBefore" v-model="agent.skipIfRanBefore" :disabled="disabledIsNotBySubdomain">
      <label class="form-check-label" for="skipIfRanBefore">
        Skip If Ran Before
      </label>
    </div>
    <div class="form-group" v-if="!isNew">
      <label for="inputArguments">Script <a href="https://docs.reconness.com/agents/script-agent">Learn more</a></label>
      <editor v-model="content" @init="editorInit" lang="csharp" theme="dracula" width="800" height="600"></editor>
    </div>
    <div class="form-group">
      <button class="btn btn-primary" v-if="isNew" v-on:click="onSave()" :disabled='!isValid()'>Add</button>
      <button class="mt-2 btn btn-primary" v-if="!isNew" v-on:click="onUpdate()">Update</button>
      <button class="mt-2 ml-2 btn btn-danger" v-if="!isNew" v-on:click="onDelete()">Delete</button>
    </div>
  </div>
</template>

<script>
  import VueTagsInput from '@johmun/vue-tags-input';

  export default {
    name: 'AgentForm',
    components: {
      editor: require('vue2-ace-editor'),
      VueTagsInput,
    },
    props: {
      parentAgent: {
        type: Object,
        required: true
      }
    },
    data: () => {
      return {
        tag: '',
        tags: [],
        autocompleteItems: [],
        agent: {},
        currentAgentName: '',
        isNew: true,
        content: null,
        disabledIsNotBySubdomain: true
      }
    },
    computed: {
      filteredItems() {
        return this.autocompleteItems.filter(i => {
          return i.text.toLowerCase().indexOf(this.tag.toLowerCase()) !== -1;
        });
      },
     },
    async mounted() {
      this.autocompleteItems = (await this.$api.get('categories')).data.map(category => {
        return { text: category.name };
      })
      this.agent = this.parentAgent || {}      
      this.isNew = this.agent.name === undefined
      this.disabledIsNotBySubdomain = !this.agent.isBySubdomain

      if (!this.isNew) {
        this.currentAgentName = this.agent.name
        this.content = this.agent.script
        this.tags = this.agent.categories.map(c => {
          return { text: c };
        })
      }
    },
    methods: {
      async onSave() {
        this.agent.categories = this.tags.map(tag => tag.text)
        await this.$api.create('agents', this.agent)
        this.$router.push({ name: 'agent', params: { agentName: this.agent.name } })
      },
      async onUpdate() {
        this.agent.categories = this.tags.map(tag => tag.text)
        this.agent.script = this.content

        await this.$api.update('agents', this.agent.id, this.agent)

        alert("The agent script code was saved")        

        if (this.currentAgentName !== this.agent.name) {
          this.$router.push({ name: 'agent', params: { agentName: this.agent.name } })
        }
        else {
          this.$router.go()
        }
      },
      async onDelete() {
        if (confirm('Are you sure to delete this agent: ' + this.agent.name)) {          
          await this.$api.delete('agents', this.agent.name)
          this.$router.go()
        }
      },
      editorInit: function () {
        require('brace/ext/language_tools')     
        require('brace/mode/csharp') 
        require('brace/theme/dracula')
        require('brace/snippets/csharp') //snippet
      },      
      isValid() {
        return this.agent.name && this.agent.command
      },
      onBySubdomain() {
        this.$refs["onlyIfIsAlive"].checked = false
        this.$refs["onlyIfHasHttpOpen"].checked = false
        this.$refs["skipIfRanBefore"].checked = false
        this.disabledIsNotBySubdomain = this.agent.isBySubdomain
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>