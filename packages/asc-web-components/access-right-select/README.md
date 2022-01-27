# AccessRightSelect

### Usage

```js
import AccessRightSelect from "@appserver/components/AccessRightSelect";
```

```jsx
<AccessRightSelect
  options={options}
  onSelect={(option) => console.log("selected", option)}
  selectedOption={{
    key: "key1",
    title: "Room administrator",
    description: `Administration of rooms, archiving of rooms, inviting and managing users in rooms.`,
    icon: CrownIcon,
    quota: "free",
    color: "#20D21F",
  }}
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
    description: "Administration of rooms, archiving of rooms, inviting and managing users in rooms.",
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
