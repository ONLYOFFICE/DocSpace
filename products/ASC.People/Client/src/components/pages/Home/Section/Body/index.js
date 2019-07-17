import React from 'react';
import { Container, Row, Col } from "reactstrap";
import {ContentRow, Link} from 'asc-web-components';

const peopleContent = (
    userName,
    department,
    phone,
    email,
    headDepartment,
    status
  ) => {
    return (
      <Container fluid={true}>
        <Row className="justify-content-start no-gutters">
          <Col className="col-12 col-sm-12 col-lg-4 text-truncate">
            <Link
              style={
                status === "pending" ? { color: "#A3A9AE" } : { color: "#333333" }
              }
              type="action"
              title={userName}
              text={userName}
              isBold={true}
              fontSize={15}
              onClick={() => console.log("User name action")}
            />
          </Col>
          <Col
            className={`${
              headDepartment ? "col-3" : "col-auto"
            } col-sm-auto col-lg-2 text-truncate`}
          >
            <Link
              style={
                status === "pending" ? { color: "#D0D5DA" } : { color: "#A3A9AE" }
              }
              type="action"
              isHovered
              title={headDepartment ? "Head of department" : ""}
              text={headDepartment ? "Head of department" : ""}
              onClick={() => console.log("Head of department action")}
            />
          </Col>
          <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
            {headDepartment && (
              <span className="d-lg-none" style={{ margin: "0 4px" }}>
                {department.title ? "|" : ""}
              </span>
            )}
            <Link
              style={
                status === "pending" ? { color: "#D0D5DA" } : { color: "#A3A9AE" }
              }
              type="action"
              isHovered
              title={department.title}
              text={department.title}
              onClick={department.action}
            />
          </Col>
          <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
            {department.title && (
              <span className="d-lg-none" style={{ margin: "0 4px" }}>
                {phone.title ? "|" : ""}
              </span>
            )}
            <Link
              style={
                status === "pending" ? { color: "#D0D5DA" } : { color: "#A3A9AE" }
              }
              type="action"
              title={phone.title}
              text={phone.title}
              onClick={phone.action}
            />
          </Col>
          <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
            {phone.title && (
              <span className="d-lg-none" style={{ margin: "0 4px" }}>
                {email.title ? "|" : ""}
              </span>
            )}
            <Link
              style={
                status === "pending" ? { color: "#D0D5DA" } : { color: "#A3A9AE" }
              }
              type="action"
              isHovered
              title={email.title}
              text={email.title}
              onClick={email.action}
            />
          </Col>
        </Row>
      </Container>
    );
  };

const SectionBodyContent = ({ users, onSelect/*, isHeaderChecked*/ }) => {
    //const [isChecked, toggleChecked] = useState(false);
    //console.log("Body isHeaderChecked=", isHeaderChecked);
    return (
      <>
        {users.map((user, index) => (
          <ContentRow
            key={user.id}
            status={user.status}
            checked={false} //{isHeaderChecked}
            data={user}
            onSelect={(checked, data) => {
              //toggleChecked(e.target.checked);
              console.log("ContentRow onSelect", checked, data);
              onSelect(checked, data);
            }}
            avatarRole={user.role}
            avatarSource={user.avatar}
            avatarName={user.userName}
            contextOptions={user.contextOptions}
          >
            {peopleContent(
              user.userName,
              user.departments[0],
              user.phones[0],
              user.emails[0],
              user.isHead,
              user.status
            )}
          </ContentRow>
        ))}
      </>
    );
  };

  export default SectionBodyContent;