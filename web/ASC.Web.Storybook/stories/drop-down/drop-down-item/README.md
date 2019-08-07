# DropDownItem

## Usage

```js
import { DropDownItem } from 'asc-web-components';
```

#### Description

Is a item of DropDown component


To add preview of user profile, you must use DropDownProfileItem component inherited from DropDownItem and add isUserPreview parameter to DropDown.

To add an avatar username and email when you turn on isUserPreview parameter, you need to add parameters of Avatar component: avatarSource - link to user's avatar, avatarRole - user's role, displayName - user name and email - user’s email address.

like:
```js
import { DropDownProfileItem } from 'asc-web-components';

<DropDownProfileItem
    avatarRole='admin'
    avatarSource=''
    displayName='Jane Doe'
    email='janedoe@gmail.com' />
```

#### Usage

```js
<DropDownItem 
    isSeparator={false} 
    isUserPreview={false} 
    isHeader={false} 
    label='Button 1' 
    icon='NavLogoIcon' 
    onClick={() => console.log('Button 1 clicked')} />
```

#### Properties

| Props              | Type     | Required | Values                      | Default        | Description                                                       |
| ------------------ | -------- | :------: | --------------------------- | -------------- | ----------------------------------------------------------------- |
| `isSeparator`      | `bool`   |    -     | -                           | `false`        | Tells when the dropdown item should display like separator        |
| `isHeader`         | `bool`   |    -     | -                           | `false`        | Tells when the dropdown item should display like header           |
| `label`            | `string` |    -     | -                           | `Dropdown item`| Dropdown item text                                                |
| `icon`             | `string` |    -     | -                           | -              | Dropdown item icon                                                |
| `onClick`          | `func`   |    -     | -                           | -              | What the dropdown item will trigger when clicked                  |
| `disabled`         | `bool`   |    -     | -                           | `false`        | Tells when the dropdown item should display like disabled         |