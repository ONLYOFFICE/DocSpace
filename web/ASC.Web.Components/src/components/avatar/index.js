import React, { memo } from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'
import { Icons } from '../icons'
import Link from '../link'

const whiteColor = '#FFFFFF';
const avatarBackground = '#ECEEF1';
const namedAvatarBackground = '#2DA7DB';

const noneUserSelect = css`
    -webkit-touch-callout: none;
    -webkit-user-select: none;
    -moz-user-select: none;
    -ms-user-select: none;
    user-select: none;
`;

const StyledAvatar = styled.div`
    position: relative;
    width: ${props =>
    (props.size === 'max' && '160px') ||
    (props.size === 'big' && '82px') ||
    (props.size === 'medium' && '48px') ||
    (props.size === 'small' && '32px')
  };
    height: ${props =>
    (props.size === 'max' && '160px') ||
    (props.size === 'big' && '82px') ||
    (props.size === 'medium' && '48px') ||
    (props.size === 'small' && '32px')
  };

    font-family: 'Open Sans',sans-serif,Arial;
    font-style: normal;
`;

const RoleWrapper = styled.div`
    position: absolute;
    left: ${props =>
    (props.size === 'max' && '0px') ||
    (props.size === 'big' && '0px') ||
    (props.size === 'medium' && '-2px') ||
    (props.size === 'small' && '-2px')
  };
    bottom: ${props =>
    (props.size === 'max' && '0px') ||
    (props.size === 'big' && '5px') ||
    (props.size === 'medium' && '3px') ||
    (props.size === 'small' && '3px')
  };
    width: ${props =>
    (props.size === 'max' && '24px') || '12px'};
    height: ${props =>
    (props.size === 'max' && '24px') || '12px'};
`;

const ImageStyled = styled.img`
    width: 100%; 
    height: auto;
    border-radius: 50%;

    ${noneUserSelect}
`;

const AvatarWrapper = styled.div`
    border-radius: 50%;
    height: 100%;
    background-color: ${props =>
    (props.source === ''
      && props.userName !== ''
      && namedAvatarBackground)
    || avatarBackground};

    & > svg {
        display: block;
        width: 50% !important;
        height: 100% !important;
        margin: auto;
    }
`;

const NamedAvatar = styled.div`
    position: absolute;
    left: 50%;
    top: 50%;
    transform: translate(-50%, -50%);
    font-weight: 600;
    font-size: ${props =>
    (props.size === 'max' && '72px') ||
    (props.size === 'big' && '34px') ||
    (props.size === 'medium' && '20px') ||
    (props.size === 'small' && '12px')
  };
    color: ${whiteColor};

    ${noneUserSelect}
`;

const EditContainer = styled.div`
    box-sizing: border-box;
    position: absolute;
    width: 100%;
    height: 100%;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    padding: 75% 16px 5px;
    text-align: center;
    line-height: 19px;
    border-radius: 50%;
    background: ${props => props.gradient ? "linear-gradient(180deg, rgba(6, 22, 38, 0) 24.48%, rgba(6, 22, 38, 0.75) 100%)" : "transparent"}; 
`;

const EmptyIcon = styled(Icons.CameraIcon)`
    border-radius: 50%;
`;

const EditLink = styled.div`
  padding-left: 10px;
  padding-right: 10px;

  a:hover {
      border-bottom: none
  }

  span {
    display: inline-block;
    max-width: 100%;
    text-decoration: underline dashed;
  }
`;

const getRoleIcon = role => {
  switch (role) {
    case 'guest':
      return <Icons.GuestIcon size='scale' />;
    case 'admin':
      return <Icons.AdministratorIcon size='scale' />;
    case 'owner':
      return <Icons.OwnerIcon size='scale' />;
    default:
      return null;
  }
};

const getInitials = userName =>
  userName
    .split(/\s/)
    .reduce((response, word) => response += word.slice(0, 1), '')
    .substring(0, 2);

const Initials = props => (
  <NamedAvatar {...props}>{getInitials(props.userName)}</NamedAvatar>
);

Initials.propTypes = {
  userName: PropTypes.string
};

// eslint-disable-next-line react/display-name
const Avatar = memo(props => {
  //console.log("Avatar render");
  const { size, source, userName, role, editing, editLabel, editAction } = props;

  const avatarContent = source
    ? <ImageStyled src={source} />
    : userName
      ? <Initials userName={userName} size={size} />
      : <EmptyIcon size='scale' />;

  const roleIcon = getRoleIcon(role);

  return (
    <StyledAvatar {...props}>
      <AvatarWrapper source={source} userName={userName}>
        {avatarContent}
      </AvatarWrapper>
      {editing && (size === 'max') &&
        <EditContainer gradient={!!source}>
          <EditLink>
            <Link
              type='action'
              title={editLabel}
              isTextOverflow={true}
              isHovered={true}
              fontSize='14px'
              fontWeight={600}
              color={whiteColor}
              onClick={editAction}
            >
              {editLabel}
            </Link>
          </EditLink>
        </EditContainer>}
      <RoleWrapper size={size}>
        {roleIcon}
      </RoleWrapper>
    </StyledAvatar>
  );
});

Avatar.propTypes = {
  size: PropTypes.oneOf(['max', 'big', 'medium', 'small']),
  role: PropTypes.oneOf(['owner', 'admin', 'guest', 'user']),
  source: PropTypes.string,
  editLabel: PropTypes.string,
  userName: PropTypes.string,
  editing: PropTypes.bool,
  editAction: PropTypes.func,
  
  className: PropTypes.string,
  id: PropTypes.string,
  style: PropTypes.oneOfType([PropTypes.object, PropTypes.array])
};

Avatar.defaultProps = {
  size: 'medium',
  role: '',
  source: '',
  editLabel: 'Edit photo',
  userName: '',
  editing: false
};

export default Avatar;