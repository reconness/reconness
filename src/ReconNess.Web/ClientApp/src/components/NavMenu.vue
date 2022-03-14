<template>
    <header>
        <nav class='navbar navbar-expand-sm navbar-expand-md navbar-dark bg-dark border-bottom box-shadow mb-3'>
            <div class="container">
                <img src="../assets/logo.png" width="50" height="50" />
                <a class="navbar-brand" href='/'>ReconNess v{{currentVersion}}</a>
                <a class="navbar-brand text-primary" href="https://github.com/reconness/reconness/blob/master/CHANGELOG.md" target="_blank">[CHANGELOG]</a>
                <a class="navbar-brand text-danger" href='https://github.com/reconness/reconness/releases/latest' target="_blank">[Latest v{{latestVersion}}]</a>
                <button class="navbar-toggler" type="button" data-toggle="collapse" data-target=".navbar-collapse" aria-label="Toggle navigation"
                        aria-expanded="isExpanded" v-on:click="toggle">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex flex-sm-row-reverse">
                    <ul class="navbar-nav flex-grow">
                        <li class="nav-item dropdown">
                            <a class="nav-link dropdown-toggle" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                Targets
                            </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdown">
                                <router-link class="dropdown-item" to="/targets/create">New Target</router-link>
                                <div class="dropdown-divider"></div>
                                <div v-for="t in targets" v-bind:key="t.id">
                                    <router-link class="dropdown-item text-success" :to="{name: 'target', params: { targetName: t.name }}">{{ t.name }}</router-link>
                                    <div v-for="rootDomain in t.rootDomains" v-bind:key="rootDomain.id">
                                        <a class="dropdown-item" :href="$router.resolve({name: 'targetRootDomain', params: { targetName: t.name, rootDomain: rootDomain.name }}).href">> {{ rootDomain.name }}</a>
                                    </div>
                                </div>
                            </div>
                        </li>
                        <li class="nav-item dropdown">
                            <a class="nav-link dropdown-toggle" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                Agents
                            </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdown">
                                <router-link class="dropdown-item" to="/agents/create">New Agent</router-link>
                                <router-link class="dropdown-item" to="/agents/debug">Debug Agent</router-link>
                                <router-link class="dropdown-item" to="/agents/install">Install Agents</router-link>
                                <div class="dropdown-divider"></div>
                                <router-link class="dropdown-item" to="/agents/wordlists">Wordlists</router-link>
                                <div class="dropdown-divider"></div>
                                <div v-for="a in agents" v-bind:key="a.id">
                                    <router-link class="dropdown-item" :to="{name: 'agentEdit', params: { agentName: a.name }}">{{ a.name }}</router-link>
                                </div>
                            </div>
                        </li>
                        <li class="nav-item dropdown" v-if="isAuth()">
                            <a class="nav-link dropdown-toggle" id="navbarDropdown" role="button" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                Account
                            </a>
                            <div class="dropdown-menu" aria-labelledby="navbarDropdown">
                                <router-link class="dropdown-item" to="/users">Users</router-link>
                                <router-link class="dropdown-item" to="/notifications">Notifications</router-link>
                                <router-link class="dropdown-item" to="/logs">Logs</router-link>
                                <div class="dropdown-divider"></div>
                                <div>
                                    <a href="#" class="dropdown-item" v-on:click="onLogout()">Sign Out</a>
                                </div>
                            </div>
                        </li>
                        <li class="nav-item" v-else>
                            <a class="nav-link" href="/">
                                Sign In
                            </a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
</template>

<script>
    import { mapState } from 'vuex'

    export default {
        name: 'NavMenu',
        data: () => {
            return {
                isExpanded: false,
                currentVersion: '',
                latestVersion: ''
            }
        },
        computed: mapState({
            targets: state => state.targets.targets,
            agents: state => state.agents.agents
        }),
        methods: {
            collapse: function () {
                this.isExpanded = false;
            },
            toggle: function () {
                this.isExpanded = !this.isExpanded;
            },
            onLogout() {
                localStorage.removeItem('user');
                this.$router.push({ name: 'login' })
            },
            isAuth() {
                const loggedIn = localStorage.getItem('user')
                return loggedIn !== null
            }
        },
        async mounted() {
            this.$store.dispatch('targets/targets')
            this.$store.dispatch('agents/agents')

            this.latestVersion = await this.$store.dispatch('accounts/latestVersion')
            this.currentVersion = await this.$store.dispatch('accounts/currentVersion')
        }
    }
</script>

<style scoped>
    a.navbar-brand {
        white-space: normal;
        text-align: center;
        word-break: break-all;
    }

    html {
        font-size: 14px;
    }

    @media (min-width: 768px) {
        html {
            font-size: 16px;
        }
    }

    .box-shadow {
        box-shadow: 0 .25rem .75rem rgba(0, 0, 0, .05);
    }
</style>
