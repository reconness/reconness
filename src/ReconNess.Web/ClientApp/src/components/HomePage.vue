<template>
  <div>
    <div class="jumbotron">
      <h1 class="text-center">Welcome <strong>{{username}}</strong>!!</h1>
      <hr class="my-4">
      <p>A Web App Tool to run and keep all your #recon in the same place and allow you to query your targets in an user friendly way.</p>
      <p class="lead">
        <a class="btn btn-info btn-lg" href="https://docs.reconness.com" target="_blank">Learn more</a>
      </p>
    </div>

    <h3>List of Targets</h3>
    <ul>
      <li v-for="t in targets" v-bind:key="t.id">
        <router-link :to="{name: 'target', params: { targetName: t.name }}">{{ t.name }}</router-link>
      </li>
    </ul>

    <hr />
    <h3>References and Resources</h3>
    <q>A smart person is not one that knows the answers, but one who knows where to find them...</q>
    <br />
    <br />
    <form>
      <div class="form-row align-items-center">
        <div class="col-auto w-50">
          <label class="sr-only" for="inlineFormInputGroup">Url</label>
          <input type="text" class="form-control" id="inlineFormInputGroup" placeholder="URL" v-model="newReference.url">
        </div>
        <div class="col-auto w-35">
          <label class="sr-only" for="inputCategory">Category</label>
          <vue-tags-input v-model="tag" placeholder="Category" :tags="tags" :autocomplete-items="filteredItems" @tags-changed="newTags => tags = newTags" />
        </div>
        <div class="col-auto">
          <button class="btn btn-primary" v-on:click="onSave()" :disabled='!isValid()'>Add</button>
        </div>
      </div>
    </form>

    <hr />
      <ul>
        <li class="row" v-for="r in references" v-bind:key="r.id">
          <div class="col-8">
            <a :href="r.url" target="_blank">{{r.url}}</a>
          </div>
          <div class="col-2 text-secondary">
            {{r.categories}}
          </div>
          <div class="col-2 text-secondary">
            <button type="button" class="btn btn-link" v-on:click="onDelete(r.id)">Delete</button>
          </div>
        </li>
      </ul>
  </div>
</template>

<script>
  import VueTagsInput from '@johmun/vue-tags-input';
  export default {
    name: 'HomePage',
    components: {
      VueTagsInput,
    },
    data: () => {
      return {
        tag: '',
        tags: [],
        autocompleteItems: [],
        targets: [],
        username: '',
        references: [],
        newReference: {}
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
      this.autocompleteItems = (await this.$api.get('references/categories')).data.map(category => {
        return { text: category };
      })

      this.targets = (await this.$api.get('targets')).data
      this.username = JSON.parse(localStorage.getItem('user')).userName;
      this.references = (await this.$api.get('references')).data
    },
    methods: {
      async onSave() {
        this.newReference.categories = this.tags.map(tag => tag.text).join(', ')
        await this.$api.create('references', this.newReference)
        this.references.push(this.newReference)

        this.newReference = {}
      },
      async onDelete(id) {          
        if (confirm('Are you sure to delete this reference?')) {

          await this.$api.delete('references', id)
          this.references = this.references.filter(function (value) {
            return value.id !== id;
          });
        }
      },
       isValid() {
        return this.newReference.url && this.tags.length > 0
      }
    }
  }
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>