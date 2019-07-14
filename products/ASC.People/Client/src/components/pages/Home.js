import React, {useState} from 'react';
import { PageLayout, Button, TreeMenu, TreeNode, Icons } from 'asc-web-components';

const treeData = [
  {
    key: '0-0',
    title: 'Departments',
    root: true,
    children: [
      { key: '0-0-0', title: 'Development', root: false },
      { key: '0-0-1', title: 'Direction', root: false },
      { key: '0-0-2', title: 'Marketing', root: false },
      { key: '0-0-3', title: 'Mobile', root: false },
      { key: '0-0-4', title: 'Support', root: false },
      { key: '0-0-5', title: 'Web', root: false },
    ],
  },
];

const PeopleTreeMenu = props => {
  const { data } = props;

  const [gData, setGData] = useState(data);
  const [autoExpandParent, setAutoExpandParent] = useState(true);

  const [expandedKeys, setExpandedKeys] = useState(['0-0-key', '0-0-0-key', '0-0-0-0-key']);

  const onDragStart = info => {
    info.event.persist();
  };

  const onDragEnter = info => {
    setExpandedKeys(info.expandedKeys);
  };

  const onDrop = info => {
    info.event.persist();
    const dropKey = info.node.props.eventKey;
    const dragKey = info.dragNode.props.eventKey;
    const dropPos = info.node.props.pos.split('-');
    const dropPosition = info.dropPosition - Number(dropPos[dropPos.length - 1]);

    const getItems = (data, key, callback) => {
      data.forEach((item, index, arr) => {
        if (item.key === key) {
          callback(item, index, arr);
          return;
        }
        if (item.children) {
          getItems(item.children, key, callback);
        }
      });
    };
    const data = [...gData];

    let dragObj;
    getItems(data, dragKey, (item, index, arr) => {
      arr.splice(index, 1);
      dragObj = item;
    });

    if (!info.dropToGap) {
      getItems(data, dropKey, item => {
        item.children = item.children || [];
        item.children.push(dragObj);
      });
    } else if (
      (info.node.props.children || []).length > 0 &&
      info.node.props.expanded &&
      dropPosition === 1
    ) {
      getItems(data, dropKey, item => {
        item.children = item.children || [];
        item.children.unshift(dragObj);
      });
    } else {
      let ar;
      let i;
      getItems(data, dropKey, (item, index, arr) => {
        ar = arr;
        i = index;
      });
      if (dropPosition === -1) {
        ar.splice(i, 0, dragObj);
      } else {
        ar.splice(i + 1, 0, dragObj);
      }
    }
    setGData(data);
  };
  const onExpand = expandedKeys => {
    setExpandedKeys(expandedKeys);
    setAutoExpandParent(false);
  };

  const getItems = data => {
    return data.map(item => {
      if (item.children && item.children.length) {
        return (
          <TreeNode
            title={item.title}
            key={item.key}
            icon={
              item.root ? (
                <Icons.CatalogDepartmentsIcon size="scale" isfill={true} color="dimgray" />
              ) : (
                ''
              )
            }
          >
            {getItems(item.children)}
          </TreeNode>
        );
      }
      return (
        <TreeNode
          key={item.key}
          title={item.title}
          icon={
            !item.root ? <Icons.CatalogFolderIcon size="scale" isfill={true} color="dimgray" /> : ''
          }
        ></TreeNode>
      );
    });
  };

  const switcherIcon = obj => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return <Icons.ExpanderDownIcon size="scale" isfill={true} color="dimgray" />;
    } else {
      return <Icons.ExpanderRightIcon size="scale" isfill={true} color="dimgray" />;
    }
  };

  return (
    <div style={{ width: '250px', margin: '20px' }}>
      <TreeMenu
        checkable={false}
        draggable={false}
        disabled={false}
        multiple={false}
        showIcon={true}
        defaultExpandAll={true}
        defaultExpandParent={true}
        onExpand={onExpand}
        autoExpandParent={autoExpandParent}
        expandedKeys={expandedKeys}
        onDragStart={info => onDragStart(info)}
        onDrop={info => onDrop(info)}
        onDragEnter={onDragEnter}
        switcherIcon={switcherIcon}
      >
        {getItems(gData)}
      </TreeMenu>
    </div>
  );
};

const Body = () => (
  <div>
    <h1>Hello, world!</h1>
    <p>Welcome to your new single-page application, built with:</p>
    <ul>
      <li><a href='https://get.asp.net/'>ASP.NET Core</a> and <a href='https://msdn.microsoft.com/en-us/library/67ef8sbd.aspx'>C#</a> for cross-platform server-side code</li>
      <li><a href='https://facebook.github.io/react/'>React</a> for client-side code</li>
      <li><a href='http://getbootstrap.com/'>Bootstrap</a> for layout and styling</li>
    </ul>
    <p>To help you get started, we have also set up:</p>
    <ul>
      <li><strong>Client-side navigation</strong>. For example, click <em>Counter</em> then <em>Back</em> to return here.</li>
      <li><strong>Development server integration</strong>. In development mode, the development server from <code>create-react-app</code> runs in the background automatically, so your client-side resources are dynamically built on demand and the page refreshes when you modify any file.</li>
      <li><strong>Efficient production builds</strong>. In production mode, development-time features are disabled, and your <code>dotnet publish</code> configuration produces minified, efficiently bundled JavaScript files.</li>
    </ul>
    <p>The <code>ClientApp</code> subdirectory is a standard React application based on the <code>create-react-app</code> template. If you open a command prompt in that directory, you can run <code>npm</code> commands such as <code>npm test</code> or <code>npm install</code>.</p>
  </div>
);

const articleHeaderContent = 'People';
const articleBodyContent = <PeopleTreeMenu data={treeData} />;
const sectionHeaderContent = 'People';

const Home = () => {

  return (
    <PageLayout
      articleHeaderContent={articleHeaderContent}
      articleBodyContent={articleBodyContent}
      sectionHeaderContent={sectionHeaderContent}
      sectionBodyContent={<Body />}
    />
  );

};

export default Home;
