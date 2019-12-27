import React from "react";
import PropTypes from "prop-types";
import { Avatar, DropDown, DropDownItem, DropDownProfileItem, utils, Link } from "asc-web-components";
const { handleAnyClick } = utils.event;

class ProfileActions extends React.PureComponent {
  constructor(props) {
    super(props);

    this.ref = React.createRef();

    this.state = {
      opened: props.opened,
      user: props.user,
      userActions: props.userActions
    };

    this.handleClick = this.handleClick.bind(this);
    this.toggle = this.toggle.bind(this);
    this.getUserRole = this.getUserRole.bind(this);
    this.onAvatarClick = this.onAvatarClick.bind(this);
    this.onDropDownItemClick = this.onDropDownItemClick.bind(this);

    if (props.opened) handleAnyClick(true, this.handleClick);
  }

  handleClick = e => {
    this.state.opened &&
      !this.ref.current.contains(e.target) &&
      this.toggle(false);
  };

  toggle = opened => {
    this.setState({ opened: opened });
  };

  componentWillUnmount() {
    handleAnyClick(false, this.handleClick);
  }

  componentDidUpdate(prevProps, prevState) {

    if (this.props.user !== prevProps.user) {
      this.setState({ user: this.props.user })
    }

    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }

    if (this.state.opened !== prevState.opened) {
      handleAnyClick(this.state.opened, this.handleClick);
    }
  }

  getUserRole = user => {
    let isModuleAdmin = user.listAdminModules && user.listAdminModules.length;

    if (user.isOwner) return "owner";
    if (user.isAdmin || isModuleAdmin) return "admin";
    if (user.isVisitor) return "guest";
    return "user";
  };

  onAvatarClick = () => {
    this.toggle(!this.state.opened);
  };

  onDropDownItemClick = action => {
    action.onClick && action.onClick();
    this.toggle(!this.state.opened);
  };

  render() {
    //console.log("Layout sub-component ProfileActions render");
    return (
      <div ref={this.ref}>
        <Avatar
          size="small"
          role={this.getUserRole(this.state.user)}
          source={this.state.user.avatarSmall}
          userName={this.state.user.displayName}
          onClick={this.onAvatarClick}
        />
        <DropDown
          isUserPreview
          withArrow
          directionX="right"
          open={this.state.opened}
          clickOutsideAction={this.onAvatarClick}
        >
          <DropDownProfileItem
            avatarRole={this.getUserRole(this.state.user)}
            avatarSource={this.state.user.avatarMedium}
            displayName={this.state.user.displayName}
            email={this.state.user.email}
          />
          {this.state.userActions.map(action => (
            <Link
              noHover={true}
              key={action.key}
              href={action.url}
              onClick={(e) => {
                if (e) {
                  this.onDropDownItemClick.bind(this, action);
                  e.preventDefault();
                }
              }
            }
              >
              <DropDownItem
                {...action} />
            </Link>
          ))}
        </DropDown>
      </div>
    );
  }
}

ProfileActions.propTypes = {
  opened: PropTypes.bool,
  user: PropTypes.object,
  userActions: PropTypes.array
};

ProfileActions.defaultProps = {
  opened: false,
  user: {},
  userActions: []
};

export default ProfileActions;
