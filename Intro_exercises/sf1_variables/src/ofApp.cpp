#include "ofApp.h"

void ofApp :: setup() {
	posX = 100;
	posY = 100;
	s = 50;
}

void ofApp :: update() {
	//	
}

void ofApp :: draw() {
	ofBackground(127);
	ofFill();
	ofSetColor(255);
	ofDrawEllipse(posX, posY, s, s);
	ofNoFill();
	ofSetColor(0);
	ofSetLineWidth(1);
	ofDrawEllipse(posX, posY, s, s);
}

//--------------------------------------------------------------
void ofApp :: keyPressed(int key) {
	//
}

void ofApp :: keyReleased(int key) {
	//
}

void ofApp :: mouseMoved(int x, int y) {
	//
}

void ofApp :: mouseDragged(int x, int y, int button) {
	//
}

void ofApp :: mousePressed(int x, int y, int button) {
	//
}

void ofApp :: mouseReleased(int x, int y, int button) {
	//
}

void ofApp :: mouseEntered(int x, int y) {
	//
}

void ofApp :: mouseExited(int x, int y) {
	//
}

void ofApp :: windowResized(int w, int h) {
	//
}

void ofApp :: gotMessage(ofMessage msg) {
	//
}

void ofApp :: dragEvent(ofDragInfo dragInfo) { 
	//
}
