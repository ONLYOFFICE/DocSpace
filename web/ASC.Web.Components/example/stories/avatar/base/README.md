# Avatar

## Usage

```js
import { Avatar } from 'asc-web-components';
```

#### Description

Required to display user avatar on page.

#### Usage

```js
<Avatar size='max' role='admin' source='' pending={false} disabled={false} />
```

#### Properties

| Props              | Type     | Required | Values                                    | Default   | Description                                           |
| ------------------ | -------- | :------: | ----------------------------------------- | --------- | ----------------------------------------------------- |
| `size`             | `oneOf`  |    -     | `retina`, `max`, `big`, `medium`, `small` | `medium`  | Tells what size avatar should be displayed            |
| `role`             | `oneOf`  |    -     | `owner`, `admin`, `guest`, `user`         | ` `       | Adds a user role table                                |
| `source`           | `string` |    -     | -                                         | ` `       | Avatar image source                                   |
| `pending`          | `bool`   |    -     | -                                         | `false`   | Reports account pending                               |
| `disabled`         | `bool`   |    -     | -                                         | `false`   | Reports that account is disabled                      |