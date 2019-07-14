import { SET_CURRENT_USER, SET_MODULES, SET_IS_LOADED, LOGOUT } from '../actions/actionTypes';
import isEmpty from 'lodash/isEmpty';

const initialState = {
    isAuthenticated: true,
    isLoaded: true,
    user: {
        id: '00000000-0000-0000-0000-000000000000',
        userName: 'Jane Doe',
        email: 'janedoe@gmail.com',
        isOwner: false,
        isAdmin: false,
        isVisitor: false,
        avatarSmall: '',
        avatarMedium: '',
      },
    currentModuleId: '11111111-1111-1111-1111-111111111111',
    modules: [
        {
          seporator: true,
          id: 'nav-seporator-1',
        },
        {
            id: '11111111-1111-1111-1111-111111111111',
            title: 'People',
            iconName: 'PeopleIcon',
            notifications: 0,
            url: '/products/people/',
            onClick: e => console.log('PeopleIcon Clicked', e),
            onBadgeClick: e => console.log('PeopleIconBadge Clicked', e),
          },
        {
          id: '22222222-2222-2222-2222-222222222222',
          title: 'Documents',
          iconName: 'DocumentsIcon',
          notifications: 2,
          url: '/products/documents/',
          onClick: e => console.log('DocumentsIcon Clicked', e),
          onBadgeClick: e => console.log('DocumentsIconBadge Clicked', e),
        },
        {
          id: '33333333-3333-3333-3333-333333333333',
          title: 'Chat',
          iconName: 'ChatIcon',
          notifications: 3,
          url: '/products/chat/',
          onClick: e => console.log('ChatIcon Clicked', e),
          isolateMode: true,
        },
        {
          id: '44444444-4444-4444-4444-444444444444',
          title: 'Mail',
          iconName: 'MailIcon',
          notifications: 7,
          url: '/products/mail/',
          onClick: e => console.log('MailIcon Clicked', e),
          onBadgeClick: e => console.log('MailIconBadge Clicked', e),
        },
        {
          id: '55555555-5555-5555-5555-555555555555',
          title: 'Projects',
          iconName: 'ProjectsIcon',
          notifications: 5,
          onClick: e => console.log('ProjectsIcon Clicked', e),
          onBadgeClick: e => console.log('ProjectsIconBadge Clicked', e),
        },
        {
            id: '77777777-7777-7777-7777-777777777777',
            title: 'CRM',
            iconName: 'CrmIcon',
            notifications: 0,
            onClick: e => console.log('CrmIcon Clicked', e),
            onBadgeClick: e => console.log('CrmIcon Clicked', e),
        },
        {
          seporator: true,
          id: 'nav-seporator-2',
        },
        {
          id: '66666666-6666-6666-6666-666666666666',
          title: 'Calendar',
          iconName: 'CalendarCheckedIcon',
          notifications: 0,
          onClick: e => console.log('CalendarIcon Clicked', e),
          onBadgeClick: e => console.log('CalendarIconBadge Clicked', e),
        },
      ]    
}

const auth = (state = initialState, action) => {
    switch (action.type) {
        case SET_CURRENT_USER:
            return Object.assign({}, state, {
                isAuthenticated: !isEmpty(action.user),
                user: action.user
            });
        case SET_MODULES:
            return Object.assign({}, state, {
                modules: action.modules
            });
        case SET_IS_LOADED:
            return Object.assign({}, state, {
                isLoaded: action.isLoaded
            });
        case LOGOUT:
            return initialState;
        default:
            return state;
    }
}

export default auth;