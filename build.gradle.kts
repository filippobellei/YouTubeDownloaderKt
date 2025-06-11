plugins {
    kotlin("jvm") version "2.1.10"
    application
}

application {
    mainClass.set("MainKt")
}

dependencies {
    implementation("com.github.TeamNewPipe:NewPipeExtractor:0.24.6")
    implementation("com.squareup.okhttp3:okhttp:4.12.0")
}

repositories {
    mavenCentral()
    maven {
        url = uri("https://jitpack.io")
    }
}
