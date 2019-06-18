import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'
import { Icons } from '../icons'

const StyledAvatar = styled.div`
    position: relative;
    width: ${props =>
        (props.size === 'retina' && '360px') ||
        (props.size === 'max' && '200px') ||
        (props.size === 'big' && '82px') ||
        (props.size === 'medium' && '48px') ||
        (props.size === 'small' && '32px')
      };
    height: ${props =>
        (props.size === 'retina' && '360px') ||
        (props.size === 'max' && '200px') ||
        (props.size === 'big' && '82px') ||
        (props.size === 'medium' && '48px') ||
        (props.size === 'small' && '32px')
      };
    border: 1px solid ${props => (props.size === 'max') ? '#bfbfbf' : '#c7c7c7'};
    border-radius: ${props => (props.size === 'max') ? '3px' : '0px'};
    -moz-border-radius: ${props => (props.size === 'max') ? '3px' : '0px'};
    -webkit-border-radius: ${props => (props.size === 'max') ? '3px' : '0px'};
`;

const Role = styled.div`
    position: absolute;
    left: ${props =>
        (props.size === 'retina' && '8px') ||
        (props.size === 'max' && '6px') ||
        (props.size === 'big' && '-2px') ||
        (props.size === 'medium' && '-2px') ||
        (props.size === 'small' && '-2px')
      };
    bottom: ${props =>
        (props.size === 'retina' && '8px') ||
        (props.size === 'max' && '6px') ||
        (props.size === 'big' && '4px') ||
        (props.size === 'medium' && '4px') ||
        (props.size === 'small' && '4px')
      };
    width: ${props =>
        (props.size === 'retina' && '40px') ||
        (props.size === 'max' && '22px') ||
        (props.size === 'big' && '12px') ||
        (props.size === 'medium' && '12px') ||
        (props.size === 'small' && '12px')
      };
    height: ${props =>
        (props.size === 'retina' && '40px') ||
        (props.size === 'max' && '22px') ||
        (props.size === 'big' && '12px') ||
        (props.size === 'medium' && '12px') ||
        (props.size === 'small' && '12px')
      };
`;

const Pending = styled.div`
    position: absolute;
    top: 50%;
    left: 50%;
    background: ${props => (props.size === 'retina' || props.size === 'max') ? 'none repeat scroll 0 0 white' : 'transparent'};
    border: ${props => (props.size === 'retina' || props.size === 'max') ? '1px solid #919191' : 'none'};
    color: #7D7D7D;
    opacity: ${props => (props.size === 'retina' || props.size === 'max') ? '0.9' : '1'};
    text-align: center;
    width: auto;
    border-radius: 3px;
    -moz-border-radius: 3px;
    -webkit-border-radius: 3px;
    transform: translate(-50%, -50%);
    font-weight: bold;
    padding: 42px 16px 5px;

    & > svg {
        position: absolute;
        top: ${props => (props.size === 'retina' || props.size === 'max') ? '40%' : '50%'};
        left: 50%;
        transform: translate(-50%, -50%);
    }
`;

const Disabled = styled(Pending)`
    color: #a83200;
`;

const ImageStyled = styled.img`
    max-width: 100%; 
    height: auto;
`;

const Avatar = props => {
    const {size, source, role, pending, disabled, pendingLabel, disabledLabel} = props;

    return (
        <StyledAvatar {... props}>
            {source === '' && <Icons.PeopleIcon size='scale' />}
            {source !== '' && <ImageStyled src={source} />}
            <Role {... props}>
                {role === 'guest' && <Icons.GuestIcon size='scale' />}
                {role === 'admin' && <Icons.AdministratorIcon size='scale' />}
                {role === 'owner' && <Icons.OwnerIcon size='scale' />}
            </Role>
            {pending && 
                <Pending {... props}>
                    {(size !== 'max' || size !== 'retina') && <Icons.CalendarIcon size={size === 'small' ? 'small' : 'big'}/>}
                    {(size === 'max' || size === 'retina') && pendingLabel}
                </Pending>}
            {disabled && 
                <Disabled {... props}>
                    {(size !== 'max' || size !== 'retina') && <Icons.CalendarIcon size={size === 'small' ? 'small' : 'big'} color='#a83200' />}
                    {(size === 'max' || size === 'retina') && disabledLabel}
                </Disabled>}
        </StyledAvatar>
    );
};

Avatar.propTypes = {
    size: PropTypes.oneOf(['retina', 'max', 'big', 'medium', 'small']),
    role: PropTypes.oneOf(['owner', 'admin','guest', 'user']),
    pending: PropTypes.bool,
    disabled: PropTypes.bool,
    source: PropTypes.string,
    pendingLabel: PropTypes.string,
    disabledLabel: PropTypes.string
};

Avatar.defaultProps = {
    size: 'medium',
    role: '',
    pending: false,
    disabled: false,
    source: '',
    pendingLabel: 'Pending',
    disabledLabel: 'Disabled'
};

export default Avatar;