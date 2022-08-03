# MediaViewer

Used to open media files

### Usage

```js
import MediaViewer from "@docspace/common/components/MediaViewer";
```

```jsx
<MediaViewer
  allowConvert={allowConvert}
  canDelete={canDelete}
  visible={visible}
  playlist={playlist}
  onDelete={onDelete}
  onDownload={onDownload}
  onClose={onClose}
  extsMediaPreviewed={extsMediaPreviewed}
  extsImagePreviewed={extsImagePreviewed}
/>
```

### Properties

| Props                |    Type    | Required | Values | Default | Description |
| -------------------- | :--------: | :------: | :----: | :-----: | ----------- |
| `allowConvert`       |   `bool`   |          |        | `true`  |             |
| `visible`            |   `bool`   |          |        | `false` |             |
| `canDelete`          | `function` |          |        |         |             |
| `playlist`           |  `array`   |          |        |         |             |
| `onDelete`           | `function` |          |        |         |             |
| `onDownload`         | `function` |          |        |         |             |
| `onClose`            | `function` |          |        |         |             |
| `extsMediaPreviewed` |  `array`   |          |        |         |             |
| `extsImagePreviewed` |  `array`   |          |        |         |             |
