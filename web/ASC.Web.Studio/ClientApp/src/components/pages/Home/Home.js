import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { withRouter } from "react-router";
import { Container, Col, Row, Collapse } from 'reactstrap';
import { ModuleTile } from 'asc-web-components';

const Tiles = ({ modules, isPrimary, history }) => {
    let index = 0;

    return (
        <Row>
            {
                modules.filter(m => m.isPrimary === isPrimary).map(module => (
                    <Col key={++index}>
                        <ModuleTile {...module} onClick={() => history.push(module.link) } />
                    </Col>
                ))
            }
        </Row>
    );
};

Tiles.propTypes = {
    modules: PropTypes.array.isRequired,
    isPrimary: PropTypes.bool.isRequired,
    history: PropTypes.object.isRequired
};

const Home = props => {
    const { modules, history } = props;

    return (
        <Container style={{ paddingTop: '62px' }}>
            <Tiles modules={modules} isPrimary={true} history={history} />
            <Tiles modules={modules} isPrimary={false} history={history} />
            <Collapse isOpen={!modules || !modules.length}>
                <Row style={{ margin: "23px 0 0" }}>
                    <Col sm="12" md={{ size: 6, offset: 3 }}>
                        <div className="alert alert-danger">No one modules available</div>
                    </Col>
                </Row>
            </Collapse>
        </Container>
    );
};

Home.propTypes = {
    modules: PropTypes.array.isRequired,
    history: PropTypes.object.isRequired
};

function mapStateToProps(state) {
    return {
        modules: state.auth.modules
    };
}

export default connect(mapStateToProps)(withRouter(Home));