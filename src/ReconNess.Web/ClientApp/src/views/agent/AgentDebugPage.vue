<template>
  <div>
    <h3>Debug Agent</h3>
    <div class="pt-2">
      <div class="form-group">
        <div class="form-group">
          <label for="output">Terminal one line output</label>
          <input name="output" formControlName="output" class="form-control" id="output" v-model="output">
        </div>
        <div class="form-group">
          <label for="script">Script <a href="https://docs.reconness.com/agents/debug-agent">Learn more</a></label>
          <editor v-model="content" @init="editorInit" lang="csharp" theme="dracula" width="800" height="600"></editor>
        </div>
      </div>
      <div class="form-group">
        <button class="btn btn-primary" v-on:click="onRun()" :disabled='!isValid()'>Run</button>
      </div>
      <div class="form-group">
        {{result}}
      </div>
    </div>
    </div>
</template>

<script>

  export default {
    name: 'AgentDebugPage',  
    components: {
      editor: require('vue2-ace-editor')
    },
    data: () => {
      return {      
        content: null,
        output: null,
        result: null
      }
    },
    methods: {
      editorInit: function () {
        require('brace/ext/language_tools')     
        require('brace/mode/csharp') 
        require('brace/theme/dracula')
        require('brace/snippets/csharp') //snippet
      },
      async onRun() {
        this.result = (await this.$api.create('agents/debug', { terminalOutput: this.output, script: this.content})).data
      },
      isValid() {
        return this.output && this.content
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>

</style>