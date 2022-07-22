# ToggleContent

ToggleContent allow you to adding information, which you may hide/show by clicking header

### Usage

```js
import ToggleContent from "@docspace/components/toggle-content";
```

```jsx
<ToggleContent>
  <span>Some text</span>
</ToggleContent>
```

#### Properties

| Props          |      Type      | Required | Values |   Default    | Description                 |
| -------------- | :------------: | :------: | :----: | :----------: | --------------------------- |
| `className`    |    `string`    |    -     |   -    |      -       | Accepts class               |
| `id`           |    `string`    |    -     |   -    |      -       | Accepts id                  |
| `isOpen`       |     `bool`     |    -     |   -    |   `false`    | State of component          |
| `label`        |     `text`     |    ✅    |   -    | `Some label` | Define label for header     |
| `style`        | `obj`, `array` |    -     |   -    |      -       | Accepts css style           |
| `enableToggle` |     `bool`     |    -     |   -    |    `true`    | Show/hide toggle functional |
