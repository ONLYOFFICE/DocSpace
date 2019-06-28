# Avatar

## Usage

```js
import { Avatar } from 'asc-web-components';
```

#### Description

Required to display user avatar on page.

#### Usage

```js
<Avatar size='max' role='admin' source='' userName='' editing={false} />
```

#### Properties

| Props              | Type     | Required | Values                                    | Default   | Description                                           |
| ------------------ | -------- | :------: | ----------------------------------------- | --------- | ----------------------------------------------------- |
| `size`             | `oneOf`  |    -     | `max`, `big`, `medium`, `small`           | `medium`  | Tells what size avatar should be displayed            |
| `role`             | `oneOf`  |    -     | `owner`, `admin`, `guest`, `user`         | ` `       | Adds a user role table                                |
| `source`           | `string` |    -     | -                                         | ` `       | Avatar image source                                   |
| `userName`         | `string` |    -     | -                                         | ` `       | If you want to create an avatar with initials. Only the first letter of the first two words of the line is used.|
| `editing`          | `bool`   |    -     | -                                         | `false`   | Displays avatar edit layer                            |
