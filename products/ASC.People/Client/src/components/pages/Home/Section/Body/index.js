import React from "react";
import { connect } from "react-redux";
import { ContentRow } from "asc-web-components";
import UserContent from "./userContent";
import _ from "lodash";

const getUserDepartment = user => {
  return {
    title: user.department,
    action: () => console.log("Department action")
  };
};

const getUserPhone = user => {
  return {
    title: user.mobilePhone,
    action: () => console.log("Phone action")
  };
};

const getUserEmail = user => {
  return {
    title: user.email,
    action: () => console.log("Email action")
  };
};

const getUserRole = user => {
  if (user.isOwner) return "owner";
  else if (user.isAdmin) return "admin";
  else if (user.isVisitor) return "guest";
  else return "user";
};

const getUserStatus = user => {
  if (user.state === 1 && user.activationStatus === 1) return "normal";
  else if (user.state === 1 && user.activationStatus === 2) return "pending";
  else if (user.state === 2) return "disabled";
  else return "normal";
};

const getUserContextOptions = user => {
  return [
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
  ];
};

const getIsHead = user => {
  return false;
};

class SectionBodyContent extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      people: this.getPeople(),
      checkedItems: new Map()
    };
  }

  getChecked = (item, selected) => {
    const { status } = item;
    let checked;
    switch (selected) {
      case "all":
          checked = true;
          break;
      case "active":
          checked = status === "normal";
          break;
      case "disabled":
          checked = status === "disabled";
          break;
      case "invited":
          checked = status === "pending";
          break;
      default:
        checked = false;
    }

    return checked;
  };

  getPeople() {
    return this.props.users.map(user => {
      const status = getUserStatus(user);
      return {
        user: user,
        status: status,
        role: getUserRole(user),
        contextOptions: getUserContextOptions(user, this.props.isAdmin),
        department: getUserDepartment(user),
        phone: getUserPhone(user),
        email: getUserEmail(user),
        isHead: getIsHead(user)
      };
    });
  }

  componentDidUpdate(prevProps) {
    /*console.log(`SectionBodyContent componentDidUpdate
    this.props.selected=${this.props.selected}
    prevProps.selected=${prevProps.selected}`);*/

    if (this.props.selected !== prevProps.selected) {
      this.state.people.forEach(item => {
        const checked = this.getChecked(item, this.props.selected);
        if(this.state.checkedItems.get(item.user.id) !== checked) {
          this.setState(prevState => ({ checkedItems: prevState.checkedItems.set(item.user.id, checked) }));
          this.props.onRowChange && this.props.onRowChange(checked, item.user);
        }
      });
    }
  }

  render() {
    const { isAdmin, onRowChange } = this.props;
    // console.log("SectionBodyContent props ", this.props);

    return (
      <>
        {this.state.people.map(item => {
          const user = item.user;
          return isAdmin ? (
            <ContentRow
              key={user.id}
              status={item.status}
              data={user}
              avatarRole={item.role}
              avatarSource={user.avatar}
              avatarName={user.userName}
              contextOptions={item.contextOptions}
              checked={this.state.checkedItems.get(user.id)}
              onSelect={(checked, data) => {
                // console.log("ContentRow onSelect", checked, data);
                this.setState(prevState => ({ checkedItems: prevState.checkedItems.set(data.id, checked) }));
                onRowChange && onRowChange(checked, data);
              }}
            >
              <UserContent
                userName={item.user.userName}
                department={item.department}
                phone={item.phone}
                email={item.email}
                headDepartment={item.isHead}
                status={item.user.status}
              />
            </ContentRow>
          ) : (
            <ContentRow
              key={item.user.id}
              status={item.status}
              avatarRole={item.role}
              avatarSource={item.user.avatar}
              avatarName={item.user.userName}
            >
              <UserContent
                userName={item.user.userName}
                department={item.department}
                phone={item.phone}
                email={item.email}
                headDepartment={item.isHead}
                status={item.user.status}
              />
            </ContentRow>
          );
        })}
      </>
    );
  }
}

const mapStateToProps = state => {
  return {
    isAdmin: state.auth.user.isAdmin || state.auth.user.isOwner
  };
};

export default connect(mapStateToProps)(SectionBodyContent);
