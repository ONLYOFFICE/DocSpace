import PropTypes from 'prop-types';
import styled, { css } from 'styled-components';

const iconSizes = {
  small: 12,
  medium: 16,
  big: 24,
};

const getSizeStyle = size => {
  switch (size) {
    case 'scale':
      return `
        &:not(:root) {
          width: 100%;
          height: auto;
        }
      `;
    case 'small':
    case 'medium':
    case 'big':
      return `
        width: ${iconSizes[size]}px;
        height: ${iconSizes[size]}px;
      `;
    default:
      return `
        width: ${iconSizes.big}px;
        height: ${iconSizes.big}px;
      `;
  }
};

export default function createStyledIcon(Component, displayName) {
  const StyledComponent = styled(Component)(
    props => `
    * {
      fill: ${props.color};
    }
    ${getSizeStyle(props.size)}
  `
  );
  StyledComponent.displayName = displayName;
  StyledComponent.propTypes = {
    color: PropTypes.string,
    size: PropTypes.oneOf(['small', 'medium', 'big', 'scale']),
  };
  return StyledComponent;
}