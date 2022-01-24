import React from 'react';
import PropTypes from 'prop-types';
import StyledHeader from './StyledHeader';

class Header extends React.Component {
  constructor(props) {
    super(props);
  }

  render() {
    const { children, displayType, className, style, theme } = this.props;
    return (
      <StyledHeader theme={theme} displayType={displayType} className={className} style={style}>
        {children}
      </StyledHeader>
    );
  }
}

Header.propTypes = {
  children: PropTypes.any,
  className: PropTypes.string,
  style: PropTypes.object,
  displayType: PropTypes.oneOf(['dropdown', 'aside']),
};

export default Header;
