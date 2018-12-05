package com.pixonic.battlemech.message;

import javax.xml.bind.annotation.XmlRootElement;

@XmlRootElement
public class InnerMessage extends Message {

    private float x;
    private com.pixonic.battlemech.message.OneTwoThree oneTwoThree;

    public float getX() {
        return x;
    }

    public void setX(float x) {
        this.x = x;
    }

    public com.pixonic.battlemech.message.OneTwoThree getOneTwoThree() {
        return oneTwoThree;
    }

    public void setOneTwoThree(com.pixonic.battlemech.message.OneTwoThree oneTwoThree) {
        this.oneTwoThree = oneTwoThree;
    }

}
