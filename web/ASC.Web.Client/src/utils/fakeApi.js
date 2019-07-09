import axios from 'axios';

const BASE_URL = 'http://localhost';
const PORT = '8080';
const PREFIX = 'api';
const VERSION = '2.0';
const API_URL = `${BASE_URL}:${PORT}/${PREFIX}/${VERSION}`;

function fakeResponse(data) {
    return Promise.resolve({
        data:
        {
            response: data
        }
    });
} 

export function login(data) {
    return axios.post(`${API_URL}/authentication`, data);
};

export function getModulesList() {
    let data = [
        {
            title: "Documents",
            link: "/products/files/",
            imageUrl: "images/documents240.png",
            description: "Create, edit and share documents. Collaborate on them in real-time. 100% compatibility with MS Office formats guaranteed.",
            isPrimary: true
        },
        {
            title: "People",
            link: "/products/people/",
            imageUrl: "images/people_logolarge.png",
            isPrimary: false
        }
    ];

    return fakeResponse(data);
};

export function getUser() {
    let data = {
        "index": "a",
        "type": "person",
        "id": "2881e6c6-7c9a-11e9-81fb-0242ac120002",
        "timestamp": null,
        "crtdate": null,
        "displayCrtdate": "NaN:NaN PM NaN/NaN/NaN",
        "displayDateCrtdate": "NaN/NaN/NaN",
        "displayTimeCrtdate": "NaN:NaN PM",
        "trtdate": null,
        "displayTrtdate": "",
        "displayDateTrtdate": "",
        "displayTimeTrtdate": "",
        "birthday": null,
        "userName": "",
        "firstName": "",
        "lastName": "",
        "displayName": "Administrator ",
        "email": "paul.bannov@gmail.com",
        "tel": "",
        "contacts": {
            "mailboxes": [{ "type": 0, "name": "mail", "title": "paul.bannov@gmail.com", "label": "Email", "istop": false, "val": "paul.bannov@gmail.com" }],
            "telephones": [],
            "links": [],
        }, "avatar": "/skins/default/images/default_user_photo_size_32-32.png",
        "avatarBig": "/skins/default/images/default_user_photo_size_82-82.png",
        "avatarSmall": "/skins/default/images/default_user_photo_size_32-32.png",
        "groups": [], "status": 0, "activationStatus": 0, "isActivated": false,
        "isPending": false,
        "isTerminated": false,
        "isMe": true,
        "isManager": false,
        "isPortalOwner": true,
        "isAdmin": true,
        "listAdminModules": [],
        "isVisitor": false,
        "isOutsider": false,
        "sex": "",
        "location": "",
        "title": "",
        "notes": "",
        "culture": "",
        "profileUrl": "/products/people/profile.aspx?user=administrator",
        "isLDAP": false,
        "isSSO": false
    }

    return fakeResponse(data);
};