import Vue from 'vue';
import Router from 'vue-router';

import HomePage from '@/components/home/HomePage';
import LoginPage from '@/components/login/LoginPage'

import TargetPage from '@/components/target/TargetPage';
import TargetCreatePage from '@/components/target/TargetCreatePage';

import AgentCreatePage from '@/components/agent/AgentCreatePage';
import AgentEditPage from '@/components/agent/AgentEditPage';
import AgentDebugPage from '@/components/agent/AgentDebugPage';

import SubdomainPage from '@/components/subdomain/SubdomainPage';

Vue.use(Router);

const router = new Router({
  routes: [
        { path: '/', name: 'home', component: HomePage },

        { path: '/login', name: 'login', component: LoginPage },

        { path: '/targets/create', name: 'targetCreate', component: TargetCreatePage },
        { path: '/targets/:targetName', name: 'target', component: TargetPage },

        { path: '/agents/create', name: 'agentCreate', component: AgentCreatePage },
        { path: '/agents/debug', name: 'agentDebug', component: AgentDebugPage },   
        { path: '/agents/:agentName', name: 'agentEdit', component: AgentEditPage },                

        { path: '/subdomains/:targetName/:subdomain', name: 'subdomain', component: SubdomainPage },

        { path: '*', redirect: { name: 'home' } },
  ],
});

router.beforeEach((to, from, next) => {
    // redirect to login page if not logged in and trying to access a restricted page
    const publicPages = ['/login'];
    const authRequired = !publicPages.includes(to.path);
    const loggedIn = localStorage.getItem('user'); // TODO: I need to verify if the token is valid too

    if (authRequired && !loggedIn) {
        return next('/login');
    }

    next();
})

export default router