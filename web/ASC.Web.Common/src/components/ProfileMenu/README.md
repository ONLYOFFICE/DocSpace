# ProfileMenu

### Usage

```js
import { ProfileMenu } from "asc-web-common";
```

```jsx
<ProfileMenu
  avatarRole="admin"
  avatarSource=""
  displayName="Jane Doe"
  email="janedoe@gmail.com"
/>
```

To add preview of user profile, you must use ProfileMenu component inherited from DropDownItem.

To add an avatar username and email, you need to add parameters of Avatar component: avatarSource - link to user's avatar, avatarRole - user's role, displayName - user name and email - user’s email address.

### Properties

| Props          |      Type      | Required |             Values             | Default | Description            |
| -------------- | :------------: | :------: | :----------------------------: | :-----: | ---------------------- |
| `avatarRole`   |    `oneOf`     |    -     | `owner`,`admin`,`guest`,`user` | `user`  | Adds a user role table |
| `avatarSource` |    `string`    |    -     |               -                |    -    | Avatar image source    |
| `className`    |    `string`    |    -     |               -                |    -    | Accepts class          |
| `displayName`  |    `string`    |    -     |               -                |    -    | User name for display  |
| `email`        |    `string`    |    -     |               -                |    -    | User email for display |
| `id`           |    `string`    |    -     |               -                |    -    | Accepts id             |
| `style`        | `obj`, `array` |    -     |               -                |    -    | Accepts css style      |
