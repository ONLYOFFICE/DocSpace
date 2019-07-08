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
      name: props.name,
      email: props.email,
      role: props.role,
      smallAvatar: props.smallAvatar,
      mediumAvatar: props.mediumAvatar
    };
  };

  toggle = () => {
    console.log(this.state.isOpen);
    this.setState({ isOpen: !this.state.isOpen });
    console.log(this.state.isOpen);
  };

  render() {
    return (
      <div>
        <Avatar size="small" role={this.state.role} source={this.state.smallAvatar} userName={this.state.name} onClick={this.toggle}/>
        <DropDown isUserPreview withArrow direction='right' isOpen={this.state.isOpen}>
          <DropDownItem isUserPreview role={this.state.role} source={this.state.mediumAvatar} userName={this.state.name} label={this.state.email}/>
          <DropDownItem label="Profile"/>
          <DropDownItem label="About"/>
          <DropDownItem label="Log out"/>
        </DropDown>
      </div>
    );
  }
}

ProfileActions.propTypes = {
  isOpen: PropTypes.bool
}

ProfileActions.defaultProps = {
  isOpen: false
}

export default ProfileActions