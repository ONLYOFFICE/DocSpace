import React from 'react'
import PropTypes from 'prop-types'
import Avatar from '../../avatar'
import DropDown from '../../drop-down'
import DropDownItem from '../../drop-down-item'

class ProfileActions extends React.Component {

  constructor(props) {
    super(props);

    this.state = {
      isOpen: props.isOpen,
      user: props.user,
      userActions: props.userActions
    };
  };

  toggle = () => {
    this.setState({ isOpen: !this.state.isOpen });
  };

  getUserRole = (user) => {
    if(user.isOwner) return "owner";
    if(user.isAdmin) return "admin";
    if(user.isVisitor) return "guest";
    return "user";
  };

  render() {
    return (
      <div>
        <Avatar
          size="small"
          role={this.getUserRole(this.state.user)}
          source={this.state.user.avatarSmall}
          userName={this.state.user.userName}
          onClick={this.toggle}
        />
        <DropDown
          isUserPreview
          withArrow
          direction='right'
          isOpen={this.state.isOpen}
        >
          <DropDownItem
            isUserPreview
            role={this.getUserRole(this.state.user)}
            source={this.state.user.avatarMedium}
            userName={this.state.user.userName}
            label={this.state.user.email}
          />
          {
            this.state.userActions.map(action => <DropDownItem {...action}/>)
          }
        </DropDown>
      </div>
    );
  }
}

ProfileActions.propTypes = {
  isOpen: PropTypes.bool,
  user: PropTypes.object,
  userActions: PropTypes.array
}

ProfileActions.defaultProps = {
  isOpen: false,
  user: {},
  userActions: []
}

export default ProfileActions