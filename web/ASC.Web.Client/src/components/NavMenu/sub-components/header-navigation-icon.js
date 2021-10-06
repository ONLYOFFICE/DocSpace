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
  top: -6px;
  right: -6px;
`;

const HeaderNavigationIcon = ({
  id,
  iconUrl,
  link,
  active,
  badgeNumber,
  onClick,
  onBadgeClick,
  url,
  ...rest
}) => {
  const color = active ? activeColor : baseColor;

  return (
    (url === '/products/people/' || url === '/products/files/') && (
      <div style={{ position: 'relative', width: '20px', height: '20px', marginRight: '22px' }}>
        <StyledReactSVG
          src={iconUrl}
          href={url}
          onClick={onClick}
          {...rest}
          beforeInjection={(svg) => {
            svg.setAttribute('style', `width: 20px; height: 20px;`);
            svg.setAttribute('fill', color);
          }}
          color={color}
        />

        <StyledBadge onClick={onBadgeClick} maxWidth={'12px'} />
      </div>
    )
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
