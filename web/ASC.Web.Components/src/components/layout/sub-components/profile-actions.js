import React, { useState } from 'react'
import PropTypes from 'prop-types'
import Avatar from '../../avatar'
import DropDown from '../../drop-down'
import DropDownItem from '../../drop-down-item'


const ProfileActions = props => {

  const [isOpen, toggle] = useState(props.isOpen);

  return (
    <>
      <Avatar size="small" role={props.role} source={props.smallAvatar} userName={props.name} onClick={() => { toggle(!isOpen); }} />
      <DropDown isUserPreview withArrow direction='right' isOpen={isOpen}>
        <DropDownItem isUserPreview role={props.role} source={props.mediumAvatar} userName={props.name} label={props.email}/>
        <DropDownItem label="Profile"/>
        <DropDownItem label="About"/>
        <DropDownItem label="Log out"/>
      </DropDown>
    </>
  );
}

ProfileActions.propTypes = {
  isOpen: PropTypes.bool
}

ProfileActions.defaultProps = {
  isOpen: false
}

export default ProfileActions