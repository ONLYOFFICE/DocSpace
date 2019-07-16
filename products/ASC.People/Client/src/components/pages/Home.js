import React, { useState } from "react";
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import _ from 'lodash';
import {
  PageLayout,
  MainButton,
  DropDownItem,
  TreeMenu,
  TreeNode,
  Icons
} from "asc-web-components";
import {
  ContentRow,
  Checkbox,
  Avatar,
  Link,
  ContextMenuButton
} from "asc-web-components";
import { GroupButtonsMenu } from "asc-web-components";
import { Container, Row, Col } from "reactstrap";

const treeData = [
  {
    key: "0-0",
    title: "Departments",
    root: true,
    children: [
      { key: "0-0-0", title: "Development", root: false },
      { key: "0-0-1", title: "Direction", root: false },
      { key: "0-0-2", title: "Marketing", root: false },
      { key: "0-0-3", title: "Mobile", root: false },
      { key: "0-0-4", title: "Support", root: false },
      { key: "0-0-5", title: "Web", root: false }
    ]
  }
];

const PeopleTreeMenu = props => {
  const { data } = props;

  const [gData, setGData] = useState(data);
  const [autoExpandParent, setAutoExpandParent] = useState(true);

  const [expandedKeys, setExpandedKeys] = useState([
    "0-0-key",
    "0-0-0-key",
    "0-0-0-0-key"
  ]);

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
    const dropPos = info.node.props.pos.split("-");
    const dropPosition =
      info.dropPosition - Number(dropPos[dropPos.length - 1]);

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
                <Icons.CatalogDepartmentsIcon
                  size="scale"
                  isfill={true}
                  color="dimgray"
                />
              ) : (
                ""
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
            !item.root ? (
              <Icons.CatalogFolderIcon
                size="scale"
                isfill={true}
                color="dimgray"
              />
            ) : (
              ""
            )
          }
        />
      );
    });
  };

  const switcherIcon = obj => {
    if (obj.isLeaf) {
      return null;
    }
    if (obj.expanded) {
      return (
        <Icons.ExpanderDownIcon size="scale" isfill={true} color="dimgray" />
      );
    } else {
      return (
        <Icons.ExpanderRightIcon size="scale" isfill={true} color="dimgray" />
      );
    }
  };

  return (
    <>
      <MainButton
        style={{ marginBottom: 5 }}
        isDisabled={false}
        isDropdown={true}
        text={"Actions"}
        clickAction={() => console.log("MainButton clickAction")}
      >
        <DropDownItem
          label="New employee"
          onClick={() => console.log("New employee clicked")}
        />
        <DropDownItem
          label="New quest"
          onClick={() => console.log("New quest clicked")}
        />
        <DropDownItem
          label="New department"
          onClick={() => console.log("New department clicked")}
        />
        <DropDownItem isSeparator />
        <DropDownItem
          label="Invitation link"
          onClick={() => console.log("Invitation link clicked")}
        />
        <DropDownItem
          label="Invite again"
          onClick={() => console.log("Invite again clicked")}
        />
        <DropDownItem
          label="Import people"
          onClick={() => console.log("Import people clicked")}
        />
      </MainButton>
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
    </>
  );
};

const peopleContent = (
  userName,
  department,
  phone,
  email,
  headDepartment,
  status
) => {
  return (
    <Container fluid={true}>
      <Row className="justify-content-start no-gutters">
        <Col className="col-12 col-sm-12 col-lg-4 text-truncate">
          <Link
            style={
              status === "pending" ? { color: "#A3A9AE" } : { color: "#333333" }
            }
            type="action"
            title={userName}
            text={userName}
            isBold={true}
            fontSize={15}
            onClick={() => console.log("User name action")}
          />
        </Col>
        <Col
          className={`${
            headDepartment ? "col-3" : "col-auto"
          } col-sm-auto col-lg-2 text-truncate`}
        >
          <Link
            style={
              status === "pending" ? { color: "#D0D5DA" } : { color: "#A3A9AE" }
            }
            type="action"
            isHovered
            title={headDepartment ? "Head of department" : ""}
            text={headDepartment ? "Head of department" : ""}
            onClick={() => console.log("Head of department action")}
          />
        </Col>
        <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
          {headDepartment && (
            <span className="d-lg-none" style={{ margin: "0 4px" }}>
              {department.title ? "|" : ""}
            </span>
          )}
          <Link
            style={
              status === "pending" ? { color: "#D0D5DA" } : { color: "#A3A9AE" }
            }
            type="action"
            isHovered
            title={department.title}
            text={department.title}
            onClick={department.action}
          />
        </Col>
        <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
          {department.title && (
            <span className="d-lg-none" style={{ margin: "0 4px" }}>
              {phone.title ? "|" : ""}
            </span>
          )}
          <Link
            style={
              status === "pending" ? { color: "#D0D5DA" } : { color: "#A3A9AE" }
            }
            type="action"
            title={phone.title}
            text={phone.title}
            onClick={phone.action}
          />
        </Col>
        <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
          {phone.title && (
            <span className="d-lg-none" style={{ margin: "0 4px" }}>
              {email.title ? "|" : ""}
            </span>
          )}
          <Link
            style={
              status === "pending" ? { color: "#D0D5DA" } : { color: "#A3A9AE" }
            }
            type="action"
            isHovered
            title={email.title}
            text={email.title}
            onClick={email.action}
          />
        </Col>
      </Row>
    </Container>
  );
};

const users = [
  {
    id: "1",
    userName: "Helen Walton",
    avatar: "",
    role: "owner",
    status: "normal",
    isHead: false,
    departments: [
      {
        title: "Administration",
        action: () => console.log("Department action")
      }
    ],
    phones: [
      {
        title: "+5 104 6473420",
        action: () => console.log("Phone action")
      }
    ],
    emails: [
      {
        title: "percival1979@yahoo.com",
        action: () => console.log("Email action")
      }
    ],
    contextOptions: [
      {
        key: "key1",
        label: "Send e-mail",
        onClick: () => console.log("Context action: Send e-mail")
      },
      {
        key: "key2",
        label: "Send message",
        onClick: () => console.log("Context action: Send message")
      },
      { key: "key3", isSeparator: true },
      {
        key: "key4",
        label: "Edit",
        onClick: () => console.log("Context action: Edit")
      },
      {
        key: "key5",
        label: "Change password",
        onClick: () => console.log("Context action: Change password")
      },
      {
        key: "key6",
        label: "Change e-mail",
        onClick: () => console.log("Context action: Change e-mail")
      },
      {
        key: "key7",
        label: "Disable",
        onClick: () => console.log("Context action: Disable")
      }
    ]
  },
  {
    id: "2",
    userName: "Nellie Harder",
    avatar: "",
    role: "user",
    status: "normal",
    isHead: true,
    departments: [
      {
        title: "Development",
        action: () => console.log("Department action")
      }
    ],
    phones: [
      {
        title: "+1 716 3748605",
        action: () => console.log("Phone action")
      }
    ],
    emails: [
      {
        title: "herta.reynol@yahoo.com",
        action: () => console.log("Email action")
      }
    ],
    contextOptions: [
      {
        key: "key1",
        label: "Send e-mail",
        onClick: () => console.log("Context action: Send e-mail")
      },
      {
        key: "key2",
        label: "Send message",
        onClick: () => console.log("Context action: Send message")
      },
      { key: "key3", isSeparator: true },
      {
        key: "key4",
        label: "Edit",
        onClick: () => console.log("Context action: Edit")
      },
      {
        key: "key5",
        label: "Change password",
        onClick: () => console.log("Context action: Change password")
      },
      {
        key: "key6",
        label: "Change e-mail",
        onClick: () => console.log("Context action: Change e-mail")
      },
      {
        key: "key7",
        label: "Disable",
        onClick: () => console.log("Context action: Disable")
      }
    ]
  },
  {
    id: "3",
    userName: "Alan Mason",
    avatar: "",
    role: "admin",
    status: "normal",
    isHead: true,
    departments: [
      {
        title: "Administration",
        action: () => console.log("Department action")
      }
    ],
    phones: [
      {
        title: "+3 956 2064314",
        action: () => console.log("Phone action")
      }
    ],
    emails: [
      {
        title: "davin_lindgr@hotmail.com",
        action: () => console.log("Email action")
      }
    ],
    contextOptions: [
      {
        key: "key1",
        label: "Send e-mail",
        onClick: () => console.log("Context action: Send e-mail")
      },
      {
        key: "key2",
        label: "Send message",
        onClick: () => console.log("Context action: Send message")
      },
      { key: "key3", isSeparator: true },
      {
        key: "key4",
        label: "Edit",
        onClick: () => console.log("Context action: Edit")
      },
      {
        key: "key5",
        label: "Change password",
        onClick: () => console.log("Context action: Change password")
      },
      {
        key: "key6",
        label: "Change e-mail",
        onClick: () => console.log("Context action: Change e-mail")
      },
      {
        key: "key7",
        label: "Disable",
        onClick: () => console.log("Context action: Disable")
      }
    ]
  },
  {
    id: "4",
    userName: "Michael Goldstein",
    avatar: "",
    role: "guest",
    status: "normal",
    isHead: false,
    departments: [
      {
        title: "Visitors",
        action: () => console.log("Department action")
      }
    ],
    phones: [
      {
        title: "+7 715 6018678",
        action: () => console.log("Phone action")
      }
    ],
    emails: [
      {
        title: "fidel_kerlu@hotmail.com",
        action: () => console.log("Email action")
      }
    ],
    contextOptions: [
      {
        key: "key1",
        label: "Send e-mail",
        onClick: () => console.log("Context action: Send e-mail")
      },
      {
        key: "key2",
        label: "Send message",
        onClick: () => console.log("Context action: Send message")
      },
      { key: "key3", isSeparator: true },
      {
        key: "key4",
        label: "Edit",
        onClick: () => console.log("Context action: Edit")
      },
      {
        key: "key5",
        label: "Change password",
        onClick: () => console.log("Context action: Change password")
      },
      {
        key: "key6",
        label: "Change e-mail",
        onClick: () => console.log("Context action: Change e-mail")
      },
      {
        key: "key7",
        label: "Disable",
        onClick: () => console.log("Context action: Disable")
      }
    ]
  },
  {
    id: "5",
    userName: "Robert Gardner",
    avatar: "",
    role: "user",
    status: "pending",
    isHead: false,
    departments: [
      {
        title: "Pending",
        action: () => console.log("Department action")
      }
    ],
    phones: [
      {
        title: "+0 000 0000000",
        action: () => console.log("Phone action")
      }
    ],
    emails: [
      {
        title: "robert_gardner@hotmail.com",
        action: () => console.log("Email action")
      }
    ],
    contextOptions: [
      {
        key: "key1",
        label: "Edit",
        onClick: () => console.log("Context action: Edit")
      },
      {
        key: "key2",
        label: "Invite again",
        onClick: () => console.log("Context action: Invite again")
      },
      {
        key: "key3",
        label: "Delete profile",
        onClick: () => console.log("Context action: Delete profile")
      }
    ]
  },
  {
    id: "6",
    userName: "Timothy Morphis",
    avatar: "",
    role: "user",
    status: "disabled",
    isHead: false,
    departments: [
      {
        title: "Disabled",
        action: () => console.log("Department action")
      }
    ],
    phones: [
      {
        title: "+9 641 1689548",
        action: () => console.log("Phone action")
      }
    ],
    emails: [
      {
        title: "timothy_j_morphis@hotmail.com",
        action: () => console.log("Email action")
      }
    ],
    contextOptions: [
      {
        key: "key1",
        label: "Edit",
        onClick: () => console.log("Context action: Edit")
      },
      {
        key: "key2",
        label: "Reassign data",
        onClick: () => console.log("Context action: Reassign data")
      },
      {
        key: "key3",
        label: "Delete personal data",
        onClick: () => console.log("Context action: Delete personal data")
      },
      {
        key: "key4",
        label: "Delete profile",
        onClick: () => console.log("Context action: Delete profile")
      }
    ]
  }
];

const Body = ({onSelect}) => {
  const [isChecked, toggleChecked] = useState(false);
  
  return (
    <>
      {users.map((user, index) => (
        <ContentRow
          key={`cr-${index}`}
          status={user.status}
          checkBox={
            <Checkbox
              isChecked={isChecked}
              onChange={e => {
                console.log("Item with id: " + e.target.value + " Checked!");
                toggleChecked(e.target.checked);
                onSelect(e.target.checked);
              }}
              isDisabled={false}
              value={user.id}
              id={user.id}
            />
          }
          avatar={
            <Avatar
              size="small"
              role={user.role}
              source={user.avatar}
              userName={user.userName}
            />
          }
          contextButton={
            <ContextMenuButton
              direction="right"
              getData={() => user.contextOptions}
            />
          }
        >
          {peopleContent(
            user.userName,
            user.departments[0],
            user.phones[0],
            user.emails[0],
            user.isHead,
            user.status
          )}
        </ContentRow>
      ))}
    </>
  );
};

const peopleItems = [
  {
    label: "Select",
    isDropdown: true,
    isSeparator: true,
    fontWeight: "bold",
    children: [
      <DropDownItem key="active" label="Active" />,
      <DropDownItem key="disabled" label="Disabled" />,
      <DropDownItem key="invited" label="Invited" />
    ]
  },
  {
    label: "Make employee",
    onClick: () => console.log("Make employee action")
  },
  {
    label: "Make guest",
    onClick: () => console.log("Make guest action")
  },
  {
    label: "Set active",
    onClick: () => console.log("Set active action")
  },
  {
    label: "Set disabled",
    onClick: () => console.log("Set disabled action")
  },
  {
    label: "Invite again",
    onClick: () => console.log("Invite again action")
  },
  {
    label: "Send e-mail",
    onClick: () => console.log("Send e-mail action")
  },
  {
    label: "Delete",
    onClick: () => console.log("Delete action")
  }
];

const articleHeaderContent = "People";
const articleBodyContent = <PeopleTreeMenu data={treeData} />;
const SectionHeaderContent = ({isChecked, toggleChecked}) => (
  isChecked ? 
    <div style={{margin: "0 -16px"}}>
      <GroupButtonsMenu
        checkBox={
          <Checkbox
            isChecked={isChecked}
            onChange={e => {
              console.log(e.target.value);
              toggleChecked(e.target.checked);
            }}
            isDisabled={false}
            value="Checkbox"
            id="check1"
          />
        }
        menuItems={peopleItems}
        visible={isChecked}
        moreLabel="More"
        closeTitle="Close"
      />
      </div>
      : "People" );

const Home = () => {
  const [isChecked, toggleChecked] = useState(false);
  
  return (
    <PageLayout
      articleHeaderContent={articleHeaderContent}
      articleBodyContent={articleBodyContent}
      sectionHeaderContent={<SectionHeaderContent isChecked={isChecked} toggleChecked={toggleChecked} />}
      sectionBodyContent={<Body onSelect={(checked) => { 
        console.log("Body onSelect", checked);
        toggleChecked(checked);
      }
      } />}
    />
  );
};

Home.propTypes = {
  modules: PropTypes.array.isRequired,
  history: PropTypes.object.isRequired,
  isLoaded: PropTypes.bool
};

function mapStateToProps(state) {
  return {
      modules: state.auth.modules,
      isLoaded: state.auth.isLoaded
  };
}

export default connect(mapStateToProps)(withRouter(Home));
