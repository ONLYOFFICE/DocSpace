import React, { useCallback } from "react";
import { withRouter } from "react-router";
import { Container, Row, Col } from "reactstrap";
import { Link, Icons } from "asc-web-components";
import { connect } from "react-redux";
import { getUserStatus } from "../../../../../store/people/selectors";

const UserContent = ({user, history,settings }) => {
  const { userName, displayName, headDepartment, department, mobilePhone, email  } = user;
  const status = getUserStatus(user);

  const onUserNameClick = useCallback(() => {
    console.log("User name action");
    history.push(`${settings.homepage}/view/${userName}`);
  }, [history, settings.homepage, userName]);

  const onHeadDepartmentClick = useCallback(
    () => console.log("Head of department action"),
    []
  );

  const onDepartmentClick = useCallback(
    () => console.log("Department action"),
    []
  );

  const onPhoneClick = useCallback(
    () => console.log("Phone action"),
    []
  );

  const onEmailClick = useCallback(
    () => console.log("Email action"),
    []
  );

  return (
    <Container fluid={true}>
      <Row className="justify-content-start no-gutters">
        <Col className="col-12 col-sm-12 col-lg-4 text-truncate">
          {displayName && 
          <Link
            isSemitransparent={status === "pending"}
            type="action"
            title={displayName}
            isBold={true}
            fontSize={15}
            onClick={onUserNameClick}
          >
            {displayName}
          </Link>}
          {status === "pending" && (
            <Icons.SendClockIcon
              style={{ marginLeft: "8px", marginTop: "-4px" }}
              size="small"
              isfill
              color="#3B72A7"
            />
          )}
          {status === "disabled" && (
            <Icons.CatalogSpamIcon
              style={{ marginLeft: "8px", marginTop: "-4px" }}
              size="small"
              isfill
              color="#3B72A7"
            />
          )}
        </Col>
        <Col
          className={`${
            headDepartment ? "col-3" : "col-auto"
          } col-sm-auto col-lg-2 text-truncate`}
        >
          {headDepartment &&
          <Link
            isSemitransparent={status === "pending"}
            type="action"
            isHovered
            title={headDepartment ? "Head of department" : ""}
            onClick={onHeadDepartmentClick}
          >
            {headDepartment ? "Head of department" : ""}
          </Link>
          }
        </Col>
        <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
          {headDepartment && (
            <span className="d-lg-none" style={{ margin: "0 4px" }}>
              {department ? "|" : ""}
            </span>
          )}
          {department &&
          <Link
            isSemitransparent={status === "pending"}
            type="action"
            isHovered
            title={department}
            onClick={onDepartmentClick}
          >
            {department}
          </Link>
          }
        </Col>
        <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
          {department && (
            <span className="d-lg-none" style={{ margin: "0 4px" }}>
              {mobilePhone ? "|" : ""}
            </span>
          )}
          {mobilePhone && 
          <Link
            isSemitransparent={status === "pending"}
            type="action"
            title={mobilePhone}
            onClick={onPhoneClick}
          >
            {mobilePhone}
          </Link>
          }
        </Col>
        <Col className={`col-3 col-sm-auto col-lg-2 text-truncate`}>
          {mobilePhone && (
            <span className="d-lg-none" style={{ margin: "0 4px" }}>
              {email ? "|" : ""}
            </span>
          )}
          <Link
            isSemitransparent={status === "pending"}
            type="action"
            isHovered
            title={email}
            onClick={onEmailClick}
          >
            {email}
          </Link>
        </Col>
      </Row>
    </Container>
  );
};

function mapStateToProps(state) {
  return {
    settings: state.settings
  };
}

export default connect(mapStateToProps)(withRouter(UserContent));
