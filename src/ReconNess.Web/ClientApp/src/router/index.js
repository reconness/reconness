import Vue from 'vue';
import Router from 'vue-router';

import HomePage from '@/components/HomePage';
import LoginPage from '@/components/login/LoginPage'

import TargetCreate from '@/components/target/TargetCreate';
import Target from '@/components/target/Target';

import AgentCreate from '@/components/agent/AgentCreate';
import AgentDebug from '@/components/agent/AgentDebug';
import Agent from '@/components/agent/Agent';

import Subdomain from '@/components/subdomain/Subdomain';

Vue.use(Router);

const router = new Router({
  routes: [
        { path: '/', name: 'home', component: HomePage },

        { path: '/login', name: 'login', component: LoginPage },

        { path: '/targets/create', name: 'targetCreate', component: TargetCreate },
        { path: '/targets/:targetName', name: 'target', component: Target },

        { path: '/agents/create', name: 'agentCreate', component: AgentCreate },
        { path: '/agents/debug', name: 'agentDebug', component: AgentDebug },
        { path: '/agents/:agentName', name: 'agent', component: Agent },    

        { path: '/subdomains/:targetName/:subdomain', name: 'subdomain', component: Subdomain },

        { path: '*', redirect: { name: 'home' } },
  ],
});

router.beforeEach((to, from, next) => {
    // redirect to login page if not logged in and trying to access a restricted page
    const publicPages = ['/login'];
    const authRequired = !publicPages.includes(to.path);
    const loggedIn = localStorage.getItem('user');

    if (authRequired && !loggedIn) {
        return next('/login');
    }

    next();
})

export default router