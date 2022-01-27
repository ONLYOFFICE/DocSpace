# AccessRightSelect

### Usage

```js
import AccessRightSelect from "@appserver/components/AccessRightSelect";
```

```jsx
<AccessRightSelect
  accessRightsList=[{...}, {...}, {...}]
  quotaList=[{...}, {...}, {...}]
/>
```

#### accessRightsList is an array of objects that contains the following fields:

- key
- title
- description
- icon

##### Example:

```js
  {
    key: "key1",
    title: "Room administrator",
    description: "Администрирование комнат, архивирование комнат, приглашение и управление пользователями в комнатах.",
    icon: CrownIcon,
  }
```

#### quotaList is an array of objects that contains the following fields:

- key
- accessRightKey
- quota
- color

##### Example:

```js
  {
    key: "key1",
    accessRightKey: "key1",
    quota: "free",
    color: "#20D21F",
  }
```

### Properties

| Props            |      Type      | Required | Values | Default | Description                                                        |
| ---------------- | :------------: | :------: | :----: | :-----: | ------------------------------------------------------------------ |
| `options`        | `obj`, `array` |    ✅    |   -    |    -    | List of rights                                                     |
| `onSelect`       | `obj`, `array` |    -     |   -    |    -    | Will be triggered whenever an AccessRightSelect is selected option |
| `selectedOption` |     `obj`      |    -     |   -    |    -    | The option that is selected by default                             |
