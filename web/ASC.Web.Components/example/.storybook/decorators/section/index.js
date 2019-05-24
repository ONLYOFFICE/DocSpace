import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';

const sectionStyles = {
  padding: 16,
};

const Section = props => <div style={sectionStyles}>{props.children}</div>;

Section.propTypes = { children: PropTypes.node.isRequired };

export default Section;