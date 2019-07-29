import React from 'react';
import { withRouter } from "react-router";
import { Container, Row, Col } from "reactstrap";
import { Link, Icons} from 'asc-web-components';
var config = require('../../../../../../package.json');

const UserContent = ({
    userName,
    displayName,
    department,
    phone,
    email,
    headDepartment,
    status,
    history
}) => (
      <Container fluid={true}>
        <Row className="justify-content-start no-gutters">
          <Col className="col-12 col-sm-12 col-lg-4 text-truncate">
            <Link
              style={
                status === "pending" ? { color: "#A3A9AE" } : { color: "#333333" }
              }
              type="action"
              title={displayName}
              isBold={true}
              fontSize={15}
              onClick={() => { 
                console.log("User name action"); 
                history.push(`${config.homepage}/view/${userName}`);
              }}
            >
              {displayName}
            </Link>
            {status === "pending" && <Icons.SendClockIcon style={{marginLeft: "8px", marginTop: "-4px"}} size='small' isfill color='#3B72A7' />}
            {status === "disabled" && <Icons.CatalogSpamIcon style={{marginLeft: "8px", marginTop: "-4px"}} size='small' isfill color='#3B72A7' />}
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
              onClick={() => console.log("Head of department action")}
            >
              {headDepartment ? "Head of department" : ""}
            </Link>
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
              onClick={department.action}
            >
            {department.title}
            </Link>
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
              onClick={phone.action}
            >
              {phone.title}
            </Link>
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
              onClick={email.action}
            >
              {email.title}
            </Link>
          </Col>
        </Row>
      </Container>
    );

export default withRouter(UserContent);