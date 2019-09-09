# Tree: TreeMenuNode

## Usage

```js
import { TreeNode } from 'asc-web-components';
```

#### Description

Tree menu node description

#### Usage

```js
<TreeNode data={data}/>;
```

#### Properties

| Props                  | Type                         | Required | Values                       | Default | Description                                                                                            |
| ---------------------- | -----------------------------| :------: | ---------------------------- | ------- | ------------------------------------------------------------------------------------------------------ |
| `className`            | `String`                     |    -     | -                            | -       | 	additional class to treeNode                                                                         |
| `checkable`            | `bool`                       |    -     | -                            | -       | control node checkable if Tree is checkable                                                            |
| `style`                | `Object`                     |    -     | -                            | -       |    set style to treeNode                                                                               |
| `disabled`             | `bool`                       |    -     | -                            | `false` | whether disabled the treeNode                                                                          |
| `disableCheckbox`      | `bool`                       |    -     | -                            | `false` | whether disable the treeNode' checkbox                                                                 |
| `title`                | `String/element`             |    -     | -                            | -       | tree/subTree's title                                                                                   |
| `key`                  | `bool`                       |    -     | -                            | -       | 	it's used with tree props's (default)ExpandedKeys / (default)CheckedKeys / (default)SelectedKeys. you'd better to set it, and it must be unique in the tree's all treeNodes                                  |
| `isLeaf`               | `bool`                       |    -     | -                            | `false` | whether it's leaf node                                                                                 |
| `icon`                 | `element/Function(props)`    |    -     | -                            | `false` | customize icon. When you pass component, whose render will receive full TreeNode props as component props|                                                                    



### Main Functions and use cases are:
