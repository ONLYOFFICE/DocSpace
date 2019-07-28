import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { ContentRow } from "asc-web-components";
import UserContent from "./userContent";
import config from "../../../../../../package.json";
import { selectUser, deselectUser, setSelection } from "../../../../../store/people/actions";
import { getSelectedUser } from '../../../../../store/people/selectors';

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
  if (user.status === 1 && user.activationStatus === 1) return "normal";
  else if (user.status === 1 && user.activationStatus === 2) return "pending";
  else if (user.status === 2) return "disabled";
  else return "normal";
};

const getUserContextOptions = (user, isAdmin, history) => {
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
      onClick: () => history.push(`${config.homepage}/edit/${user.userName}`)
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
  getChecked = (status, selected) => {
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

  getPeople = () => {
    return this.props.users.map(user => {
      const status = getUserStatus(user);
      return {
        user: user,
        status: status,
        role: getUserRole(user),
        contextOptions: getUserContextOptions(
          user,
          this.props.isAdmin,
          this.props.history
        ),
        department: getUserDepartment(user),
        phone: getUserPhone(user),
        email: getUserEmail(user),
        isHead: getIsHead(user)
      };
    });
  };

  componentDidUpdate(prevProps) {
    /*console.log(`SectionBodyContent componentDidUpdate
    this.props.selected=${this.props.selected}
    prevProps.selected=${prevProps.selected}`);*/

    if (this.props.selected !== prevProps.selected) {
      const { setSelection } = this.props;

      let newSelection = [];
      this.props.users.forEach(user => {
        const checked = this.getChecked(getUserStatus(user), this.props.selected);

        if(checked)
          newSelection.push(user);
      });

      setSelection(newSelection);
    }
  }

  render() {
    console.log("Home SectionBodyContent render()");
    const { isAdmin, selection, selectUser, deselectUser } = this.props;
    // console.log("SectionBodyContent props ", this.props);

    return (
      <>
        {this.getPeople().map(item => {
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
              checked={getSelectedUser(selection, user.id)}
              onSelect={(checked, data) => {
                console.log("ContentRow onSelect", checked, data);
                //this.setState(prevState => ({ checkedItems: prevState.checkedItems.set(data.id, checked) }));
                //onChange && onChange(checked, data);
                if (checked) {
                  selectUser(user);
                }
                else {
                  deselectUser(user);
                }
              }}
            >
              <UserContent
                userName={item.user.userName}
                displayName={item.user.displayName}
                department={item.department}
                phone={item.phone}
                email={item.email}
                headDepartment={item.isHead}
                status={item.status}
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
};

const mapStateToProps = state => {
  return {
    selection: state.people.selection,
    selected: state.people.selected,
    users: state.people.users,
    isAdmin: state.auth.user.isAdmin || state.auth.user.isOwner
  };
};

export default connect(
  mapStateToProps,
  { selectUser, deselectUser, setSelection }
)(withRouter(SectionBodyContent));
