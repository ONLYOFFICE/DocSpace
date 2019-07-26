import React from 'react';
import { storiesOf } from '@storybook/react';
import { ContentRow, Link, Icons} from 'asc-web-components';
import Section from '../../../.storybook/decorators/section';
import { Container, Row, Col } from 'reactstrap';

const users = [
  {
    id: '1',
    userName: 'Helen Walton',
    avatar: '',
    role: 'owner',
    status: 'normal',
    isHead: false,
    departments: [
      {
        title: 'Administration',
        action: () => console.log('Department action')
      }
    ],
    phones: [
      {
        title: '+5 104 6473420',
        action: () => console.log('Phone action')
      }
    ],
    emails: [
      {
        title: 'percival1979@yahoo.com',
        action: () => console.log('Email action')
      }
    ],
    contextOptions: [
      {key: 'key1', label: 'Send e-mail', onClick: () => console.log('Context action: Send e-mail')},
      {key: 'key2', label: 'Send message', onClick: () => console.log('Context action: Send message')},
      {key: 'key3', isSeparator: true },
      {key: 'key4', label: 'Edit', onClick: () => console.log('Context action: Edit')},
      {key: 'key5', label: 'Change password', onClick: () => console.log('Context action: Change password')},
      {key: 'key6', label: 'Change e-mail', onClick: () => console.log('Context action: Change e-mail')},
      {key: 'key7', label: 'Disable', onClick: () => console.log('Context action: Disable')}
    ]
  },
  {
    id: '2',
    userName: 'Nellie Harder',
    avatar: '',
    role: 'user',
    status: 'normal',
    isHead: true,
    departments: [
      {
        title: 'Development',
        action: () => console.log('Department action')
      }
    ],
    phones: [
      {
        title: '+1 716 3748605',
        action: () => console.log('Phone action')
      }
    ],
    emails: [
      {
        title: 'herta.reynol@yahoo.com',
        action: () => console.log('Email action')
      }
    ],
    contextOptions: [
      {key: 'key1', label: 'Send e-mail', onClick: () => console.log('Context action: Send e-mail')},
      {key: 'key2', label: 'Send message', onClick: () => console.log('Context action: Send message')},
      {key: 'key3', isSeparator: true },
      {key: 'key4', label: 'Edit', onClick: () => console.log('Context action: Edit')},
      {key: 'key5', label: 'Change password', onClick: () => console.log('Context action: Change password')},
      {key: 'key6', label: 'Change e-mail', onClick: () => console.log('Context action: Change e-mail')},
      {key: 'key7', label: 'Disable', onClick: () => console.log('Context action: Disable')}
    ]
  },
  {
    id: '3',
    userName: 'Alan Mason',
    avatar: '',
    role: 'admin',
    status: 'normal',
    isHead: true,
    departments: [
      {
        title: 'Administration',
        action: () => console.log('Department action')
      }
    ],
    phones: [
      {
        title: '+3 956 2064314',
        action: () => console.log('Phone action')
      }
    ],
    emails: [
      {
        title: 'davin_lindgr@hotmail.com',
        action: () => console.log('Email action')
      }
    ],
    contextOptions: [
      {key: 'key1', label: 'Send e-mail', onClick: () => console.log('Context action: Send e-mail')},
      {key: 'key2', label: 'Send message', onClick: () => console.log('Context action: Send message')},
      {key: 'key3', isSeparator: true },
      {key: 'key4', label: 'Edit', onClick: () => console.log('Context action: Edit')},
      {key: 'key5', label: 'Change password', onClick: () => console.log('Context action: Change password')},
      {key: 'key6', label: 'Change e-mail', onClick: () => console.log('Context action: Change e-mail')},
      {key: 'key7', label: 'Disable', onClick: () => console.log('Context action: Disable')}
    ]
  },
  {
    id: '4',
    userName: 'Michael Goldstein',
    avatar: '',
    role: 'guest',
    status: 'normal',
    isHead: false,
    departments: [
      {
        title: 'Visitors',
        action: () => console.log('Department action')
      }
    ],
    phones: [
      {
        title: '+7 715 6018678',
        action: () => console.log('Phone action')
      }
    ],
    emails: [
      {
        title: 'fidel_kerlu@hotmail.com',
        action: () => console.log('Email action')
      }
    ],
    contextOptions: [
      {key: 'key1', label: 'Send e-mail', onClick: () => console.log('Context action: Send e-mail')},
      {key: 'key2', label: 'Send message', onClick: () => console.log('Context action: Send message')},
      {key: 'key3', isSeparator: true },
      {key: 'key4', label: 'Edit', onClick: () => console.log('Context action: Edit')},
      {key: 'key5', label: 'Change password', onClick: () => console.log('Context action: Change password')},
      {key: 'key6', label: 'Change e-mail', onClick: () => console.log('Context action: Change e-mail')},
      {key: 'key7', label: 'Disable', onClick: () => console.log('Context action: Disable')}
    ]
  },
  {
    id: '5',
    userName: 'Robert Gardner',
    avatar: '',
    role: 'user',
    status: 'pending',
    isHead: false,
    departments: [
      {
        title: 'Pending',
        action: () => console.log('Department action')
      }
    ],
    phones: [
      {
        title: '+0 000 0000000',
        action: () => console.log('Phone action')
      }
    ],
    emails: [
      {
        title: 'robert_gardner@hotmail.com',
        action: () => console.log('Email action')
      }
    ],
    contextOptions: [
      {key: 'key1', label: 'Edit', onClick: () => console.log('Context action: Edit')},
      {key: 'key2', label: 'Invite again', onClick: () => console.log('Context action: Invite again')},
      {key: 'key3', label: 'Delete profile', onClick: () => console.log('Context action: Delete profile')}
    ]
  },
  {
    id: '6',
    userName: 'Timothy Morphis',
    avatar: '',
    role: 'user',
    status: 'disabled',
    isHead: false,
    departments: [
      {
        title: 'Disabled',
        action: () => console.log('Department action')
      }
    ],
    phones: [
      {
        title: '+9 641 1689548',
        action: () => console.log('Phone action')
      }
    ],
    emails: [
      {
        title: 'timothy_j_morphis@hotmail.com',
        action: () => console.log('Email action')
      }
    ],
    contextOptions: [
      {key: 'key1', label: 'Edit', onClick: () => console.log('Context action: Edit')},
      {key: 'key2', label: 'Reassign data', onClick: () => console.log('Context action: Reassign data')},
      {key: 'key3', label: 'Delete personal data', onClick: () => console.log('Context action: Delete personal data')},
      {key: 'key4', label: 'Delete profile', onClick: () => console.log('Context action: Delete profile')}
    ]
  }
];

storiesOf('EXAMPLES|ContentRow', module)
  .add('people row', () => {

    const peopleContent = (userName, department, phone, email, headDepartment, status) => {
      return(
        <Container fluid={true}>
          <Row className="justify-content-start no-gutters">
            <Col className="col-12 col-sm-12 col-lg-4 text-truncate">
              <Link style={status === 'pending' ? {color: '#A3A9AE'} : {color: '#333333'}} 
                    type='action' 
                    title={userName} 
                    text={userName} 
                    isBold={true} 
                    fontSize={15} 
                    onClick={()=> console.log('User name action')} />
                    {status === "pending" && <Icons.SendClockIcon style={{marginLeft: "8px", marginTop: "-4px"}} size='small' isfill color='#3B72A7' />}
                    {status === "disabled" && <Icons.CatalogSpamIcon style={{marginLeft: "8px", marginTop: "-4px"}} size='small' isfill color='#3B72A7' />}
            </Col>
            <Col className={`${headDepartment ? 'col-3': 'col-auto' } col-sm-auto col-lg-2 text-truncate`}>
              <Link style={status === 'pending' ? {color: '#D0D5DA'} : {color: '#A3A9AE'}} 
                    type='action'
                    isHovered
                    title={headDepartment ? 'Head of department' : ''}
                    text={headDepartment ? 'Head of department' : ''}
                    onClick={() => console.log('Head of department action')} />
            </Col>
            <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
              {headDepartment && 
                <span className="d-lg-none" style={{margin: '0 4px'}}>{department.title ? '|' : ''}</span>
              }
              <Link style={status === 'pending' ? {color: '#D0D5DA'} : {color: '#A3A9AE'}} 
                    type='action' 
                    isHovered 
                    title={department.title} 
                    text={department.title} 
                    onClick={department.action} />
            </Col>
            <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
              {department.title && 
                <span className="d-lg-none" style={{margin: '0 4px'}}>{phone.title ? '|' : ''}</span>
              }
              <Link style={status === 'pending' ? {color: '#D0D5DA'} : {color: '#A3A9AE'}}
                    type='action' 
                    title={phone.title} 
                    text={phone.title} 
                    onClick={phone.action} />
            </Col>
            <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
              {phone.title && 
                <span className="d-lg-none" style={{margin: '0 4px'}}>{email.title ? '|' : ''}</span>
              }
              <Link style={status === 'pending' ? {color: '#D0D5DA'} : {color: '#A3A9AE'}} 
                    type='action' 
                    isHovered 
                    title={email.title} 
                    text={email.title} 
                    onClick={email.action} />
            </Col>
          </Row>
        </Container>
      )
    };
   
    return (
      <Section>
        {users.map(user => {
          return (
            <ContentRow key={user.id}
                        status={user.status} 
                        checked={false}
                        data={user}
                        avatarRole={user.role} 
                        avatarSource={user.avatar} 
                        avatarName={user.userName} 
                        contextOptions={user.contextOptions}
            >
              {peopleContent(user.userName, user.departments[0], user.phones[0], user.emails[0], user.isHead, user.status)}
            </ContentRow>
          );
        })}
      </Section>
    );
  });
