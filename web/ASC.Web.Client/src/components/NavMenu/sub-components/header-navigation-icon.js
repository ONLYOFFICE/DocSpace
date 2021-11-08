import React from 'react';
import PropTypes from 'prop-types';
import styled from 'styled-components';
import { ReactSVG } from 'react-svg';
import Badge from '@appserver/components/badge';

const baseColor = '#7A95B0',
  activeColor = '#FFFFFF';

const StyledReactSVG = styled(ReactSVG)`
  position: relative;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  cursor: pointer;
  svg {
    path {
      fill: ${(props) => props.color};
    }
  }
`;

const StyledBadge = styled(Badge)`
  position: absolute;
  top: -8px;
  right: -8px;
`;

const HeaderNavigationIcon = ({
  id,
  iconUrl,
  link,
  active,
  badgeNumber,
  onItemClick,
  onBadgeClick,
  url,
  ...rest
}) => {
  const color = active ? activeColor : baseColor;
  return (
    <div style={{ position: 'relative', width: '20px', height: '20px', marginRight: '22px' }}>
      <StyledReactSVG src={iconUrl} href={url} onClick={onItemClick} {...rest} color={color} />

      <StyledBadge onClick={onBadgeClick} label={badgeNumber} maxWidth={'6px'} fontSize={'9px'} />
    </div>
  );
};

HeaderNavigationIcon.propTypes = {
  id: PropTypes.string,
  iconUrl: PropTypes.string,
  link: PropTypes.string,
  active: PropTypes.bool,
  badgeNumber: PropTypes.oneOfType([PropTypes.string, PropTypes.number]),
  onClick: PropTypes.func,
  onBadgeClick: PropTypes.func,
  url: PropTypes.string,
};

export default React.memo(HeaderNavigationIcon);
