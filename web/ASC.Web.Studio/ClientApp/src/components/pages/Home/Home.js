import React from 'react';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { Container, Col, Row, Collapse } from 'reactstrap';
import { ModuleTile } from 'asc-web-components';

const Tiles = ({ modules, isPrimary }) => {
    let index = 0;
    return (
        <Row>
            {
                modules.filter(m => m.isPrimary === isPrimary).map(module => (
                    <Col key={++index}>
                        <ModuleTile {...module} />
                    </Col>
                ))
            }
        </Row>
    );
};

Tiles.propTypes = {
    modules: PropTypes.array.isRequired,
    isPrimary: PropTypes.bool.isRequired
};

const Home = props => {
    const { modules } = props;

    return (
        <Container>
            <Tiles modules={modules} isPrimary={true} />
            <Tiles modules={modules} isPrimary={false} />
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
};

function mapStateToProps(state) {
    return {
        modules: state.auth.modules
    };
}

export default connect(mapStateToProps)(Home);