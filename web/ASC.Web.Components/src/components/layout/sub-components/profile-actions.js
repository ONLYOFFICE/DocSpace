import React from 'react'
import PropTypes from 'prop-types'
import Avatar from '../../avatar'
import DropDown from '../../drop-down'
import DropDownItem from '../../drop-down-item'

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
  };

  handleClick = (e) => {
    !this.ref.current.contains(e.target) && this.toggle(false);
  }

  toggle = (opened) => {
    this.setState({ opened: opened });
  }

  componentDidMount() {
    if (this.ref.current) {
      document.addEventListener("click", this.handleClick);
    }
  }

  componentWillUnmount() {
    document.removeEventListener("click", this.handleClick)
  }

  componentDidUpdate(prevProps) {
    if (this.props.opened !== prevProps.opened) {
      this.toggle(this.props.opened);
    }
  }

  getUserRole = (user) => {
    if(user.isOwner) return "owner";
    if(user.isAdmin) return "admin";
    if(user.isVisitor) return "guest";
    return "user";
  };

  render() {
    //console.log("Layout sub-component ProfileActions render");
    return (
      <div ref={this.ref}>
        <Avatar
          size="small"
          role={this.getUserRole(this.state.user)}
          source={this.state.user.avatarSmall}
          userName={this.state.user.userName}
          onClick={() => { 
            this.toggle(!this.state.opened);
          }}
        />
        <DropDown
          isUserPreview
          withArrow
          directionX='right'
          isOpen={this.state.opened}
        >
          <DropDownItem
            isUserPreview
            role={this.getUserRole(this.state.user)}
            source={this.state.user.avatarMedium}
            userName={this.state.user.userName}
            label={this.state.user.email}
          />
          {
            this.state.userActions.map(action => 
              <DropDownItem 
                {...action}
                onClick={() => { 
                  action.onClick && action.onClick();
                  this.toggle(!this.state.opened);
                }}
              />
            )
          }
        </DropDown>
      </div>
    );
  }
}

ProfileActions.propTypes = {
  opened: PropTypes.bool,
  user: PropTypes.object,
  userActions: PropTypes.array
}

ProfileActions.defaultProps = {
  opened: false,
  user: {},
  userActions: []
}

export default ProfileActions