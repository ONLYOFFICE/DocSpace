import React from 'react'
import styled from 'styled-components'
import PropTypes from 'prop-types'
import { Icons } from '../icons'

const whiteColor = '#FFFFFF';
const avatarBackground = '#ECEEF1';

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

    font-family: 'Open Sans';
    font-style: normal;
`;

const RoleWrapper = styled.div`
    position: absolute;
    left: ${props =>
        (props.size === 'max' && '0px') ||
        (props.size === 'big' && '-2px') ||
        (props.size === 'medium' && '0px') ||
        (props.size === 'small' && '-2px')
    };
    bottom: ${props =>
        (props.size === 'max' && '0px') ||
        (props.size === 'big' && '4px') ||
        (props.size === 'medium' && '0px') ||
        (props.size === 'small' && '3px')
    };
    width: ${props =>
        (props.size === 'max' && '24px') ||
        (props.size === 'big' && '12px') ||
        (props.size === 'medium' && '12px') ||
        (props.size === 'small' && '12px')
    };
    height: ${props =>
        (props.size === 'max' && '24px') ||
        (props.size === 'big' && '12px') ||
        (props.size === 'medium' && '12px') ||
        (props.size === 'small' && '12px')
    };
`;

const ImageStyled = styled.img`
    max-width: 100%; 
    height: auto;
    border-radius: 50%;
`;

const AvatarWrapper = styled.div`
    border-radius: 50%;
    height: 100%;
    background-color: ${avatarBackground};

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
`;

const EditContainer = styled.div`
    position: absolute;
    width: 100%;
    height: 100%;

    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);

    padding: 75% 16px 5px;
    text-align: center;
    font-size: 14px;
    line-height: 19px;

    border-radius: 50%;
    background: linear-gradient(180deg, rgba(6, 22, 38, 0) 24.48%, rgba(6, 22, 38, 0.75) 100%);
`;

const EditLink = styled.a`
    border-bottom: 1px dotted;
    font-family: Open Sans;
    font-style: normal;
    font-weight: 600;
    font-size: 14px;
    line-height: 19px;
    
    color: ${whiteColor} !important;
    
    &:hover{
        color: ${whiteColor} !important;
        cursor: pointer;
    }

    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
    -o-text-overflow: ellipsis;
    -moz-text-overflow: ellipsis;
    -webkit-text-overflow: ellipsis;
`;

const EmptyIcon = styled(Icons.CameraIcon)`
    border-radius: '50%';
`;

const getInitials = userName => {
    const initials = userName
        ? userName
            .split(/\s/)
            .reduce((response, word) => response += word.slice(0, 1), '')
            .substring(0, 2)
        : "";
    return initials;
};

const Avatar = React.memo(props => {
    //console.log("Avatar render");
    const { size, source, userName, role, editing, editLabel, editAction } = props;

    const initials = getInitials(userName);

    return (
        <StyledAvatar {...props}>
            <AvatarWrapper {...props}>
                {source !== '' && <ImageStyled src={source} />}
                {(source === '' && userName !== '') && <NamedAvatar {...props}>{initials}</NamedAvatar>}
                {(source === '' && userName === '') && <EmptyIcon size='scale' />}
            </AvatarWrapper>
            {editing && (size === 'max') &&
                <EditContainer {...props}>
                    <EditLink title={editLabel} onClick={editAction}>{editLabel}</EditLink>
                </EditContainer>}
            <RoleWrapper {...props}>
                {role === 'guest' && <Icons.GuestIcon size='scale' />}
                {role === 'admin' && <Icons.AdministratorIcon size='scale' />}
                {role === 'owner' && <Icons.OwnerIcon size='scale' />}
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
    editing: PropTypes.bool
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