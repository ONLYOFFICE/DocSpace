import React from 'react'
import styled, { css } from 'styled-components'
import PropTypes from 'prop-types'

import device from '../../device'
import Link from '../../link'
import { Icons } from '../../icons'

const truncateCss = css`
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
`;

const commonCss = css`
  margin: 0 8px;
  font-family: Open Sans;
  font-size: 12px;
  font-style: normal;
  font-weight: 600;
`;

const RowContainer = styled.div`
  width: 100%
  height: 50px;
  display: inline-flex;

  @media ${device.tablet} {
    display: block;
  }
`;

const NameWrapper = styled.div`
  ${commonCss};

  display: flex;
  align-self: center;
  margin-right: auto;
  min-width: 140px;

  @media ${device.tablet} {
    min-width: 140px;
    margin-right: 8px;
    margin-top: 6px;
  }
`;

const Name = styled.div`
  ${truncateCss};
  height: 20px;
  margin-right: 8px;
`;

const StatusIcon = styled.div`
    align-self: center;
 `;

const DepartmentWrapper = styled.div`
  ${truncateCss};
  ${commonCss};

  align-self: center;
  width: 160px;
  color: ${props => props.color && props.color};

  @media ${device.tablet} {
    display: none;
  }
`;

const PhoneWrapper = styled.div`
  ${truncateCss};
  ${commonCss};

  align-self: center;
  width: 100px;
  color: ${props => props.color && props.color};

  @media ${device.tablet} {
    display: none;
  }
`;

const EmailWrapper = styled.div`
  ${truncateCss};
  ${commonCss};

  align-self: center;
  width: 160px;
  color: ${props => props.color && props.color};

  @media ${device.tablet} {
    display: none;
  }
`;

const TabletSideInfo = styled.div`
  display: none;

  @media ${device.tablet} {

    display: block;
    min-width: 160px;
    margin: 0 8px;
    color: ${props => props.color && props.color};

    ${commonCss};
    ${truncateCss};
  }
`;

const PeopleRow = props => {
  const { status, displayName, department, phone, email, onDisplayNameClick, onDepartmentClick, onPhoneClick, onEmailClick } = props;

  const nameColor = status === 'pending' ? '#A3A9AE' : '#333333';
  const sideInfoColor = status === 'pending' ? '#D0D5DA' : '#A3A9AE';

  return (
    <RowContainer>
      <NameWrapper>
        <Name>
          {displayName && displayName !=='' && 
            <Link
              type='page'
              title={displayName}
              isBold={true}
              fontSize={15}
              color={nameColor}
              onClick={onDisplayNameClick}
            >
              {displayName}
            </Link>
          }
        </Name>
        <StatusIcon>
          {status === 'pending' &&
            <Icons.SendClockIcon
              size='small'
              isfill={true}
              color='#3B72A7'
            />
          }
          {status === 'disabled' &&
            <Icons.CatalogSpamIcon
              size='small'
              isfill={true}
              color='#3B72A7'
            />
          }
        </StatusIcon>
      </NameWrapper>
      <DepartmentWrapper>
        {department && department !== '' && 
          <Link
            type='action'
            title={department}
            fontSize={12}
            color={sideInfoColor}
            onClick={onDepartmentClick}
          >
            {department}
          </Link>
        }
      </DepartmentWrapper>
      <PhoneWrapper color={sideInfoColor}>
        {phone && phone !== '' && 
          <Link
            type='page'
            title={email}
            fontSize={12}
            color={sideInfoColor}
            onClick={onPhoneClick}
          >
            {phone}
          </Link>
        }
      </PhoneWrapper>
      <EmailWrapper color={sideInfoColor}>
        {email && email !== '' && 
          <Link
            type='page'
            title={email}
            fontSize={12}
            color={sideInfoColor}
            onClick={onEmailClick}
          >
            {email}
          </Link>
        }
      </EmailWrapper>
      <TabletSideInfo color={sideInfoColor}>
        {department && department !== '' && department + ' | '}
        {phone && phone !== '' && phone + ' | '}
        {email && email !== '' && email}
      </TabletSideInfo>
    </RowContainer>
  )
};

PeopleRow.propTypes = {
  status: PropTypes.string, 
  displayName: PropTypes.string, 
  department: PropTypes.string, 
  phone: PropTypes.string, 
  email: PropTypes.string, 
  onDisplayNameClick: PropTypes.func, 
  onDepartmentClick: PropTypes.func, 
  onPhoneClick: PropTypes.func, 
  onEmailClick: PropTypes.func
};

export default PeopleRow;