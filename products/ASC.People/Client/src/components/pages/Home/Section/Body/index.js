import React from "react";
import { withRouter } from "react-router";
import { connect } from "react-redux";
import { ContentRow } from "asc-web-components";
import UserContent from "./userContent";
//import config from "../../../../../../package.json";
import { selectUser, deselectUser, setSelection } from "../../../../../store/people/actions";
import { isSelected, convertPeople } from '../../../../../store/people/selectors';
import { isAdmin } from '../../../../../store/auth/selectors';

class SectionBodyContent extends React.PureComponent {
  render() {
    console.log("Home SectionBodyContent render()");
    const { users, isAdmin, selection, selectUser, deselectUser, history, settings} = this.props;

    return (
      <>
        {convertPeople(users, isAdmin, history, settings).map(item => {
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
              checked={isSelected(selection, user.id)}
              onSelect={(checked, data) => {
                console.log("ContentRow onSelect", checked, data);
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
    isAdmin: isAdmin(state.auth),
    settings: state.settings
  };
};

export default connect(
  mapStateToProps,
  { selectUser, deselectUser, setSelection }
)(withRouter(SectionBodyContent));
