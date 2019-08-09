import React from 'react';
import PropTypes from "prop-types";
import GroupForm from './Form/groupForm'


const SectionBodyContent = (props) => {
  const {group} = props;

  return (
    <GroupForm initialValues={group} />
  );
};

SectionBodyContent.propTypes = {
  group: PropTypes.object
};

SectionBodyContent.defaultProps = {
  group: null
}

export default SectionBodyContent;