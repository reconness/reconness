<template>
  <div>
    <h3>References and Resources</h3>
    <q>A smart person is not one that knows the answers, but one who knows where to find them...</q>

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
          <button class="btn btn-primary" v-on:click.prevent="onSave()" :disabled='!isValid()'>Add</button>
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
          <button type="button" class="btn btn-link" v-on:click="onDelete(r)">Delete</button>
        </div>
      </li>
    </ul>
  </div>
</template>

<script>

  import { mapState } from 'vuex'
  import VueTagsInput from '@johmun/vue-tags-input';

  export default {
    name: 'ReferenceAndResource',
    components: {
      VueTagsInput,
    },
    data: () => {
      return {
        newReference: {},
        tag: '',
        tags: [],
        autocompleteItems: []
      }
    },
    computed: {
      ...mapState({
        references: state => state.references.references
      }),
      filteredItems() {
        return this.autocompleteItems.filter(i => {
          return i.text.toLowerCase().indexOf(this.tag.toLowerCase()) !== -1;
        });
      },
    },
    async mounted() {
      this.$store.dispatch('references/categories')
        .then((categories) => {
          this.autocompleteItems = categories.map(category => {
            return { text: category };
          })
        })

      this.$store.dispatch('references/references')
    },
    methods: {
      async onSave() {
        this.newReference.categories = this.tags.map(tag => tag.text).join(', ')

        this.$store.dispatch('references/createReference', this.newReference)
          .then(() => {
            this.newReference = {}
            this.tags = []

            alert("The reference was updated")
          })
          .catch(error => {
            if (error) {
              alert(JSON.stringify(error))
            } else {
              alert("The Reference cannot be added. Try again, please!")
            }
          })
      },
      async onDelete(reference) {
        if (confirm('Are you sure to delete this reference?')) {
          this.$store.dispatch('references/deleteReference', reference)
            .then(() => {
              alert("The Reference was deleted")
            })
            .catch(error => {
              if (error) {
                alert(JSON.stringify(error))
              }
              else {
                alert("The Reference cannot be deleted. Try again, please!")
              }
            })
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